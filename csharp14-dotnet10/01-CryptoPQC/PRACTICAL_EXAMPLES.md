# Post-Quantum Cryptography: Practical Examples

This file contains practical code examples for common scenarios using .NET 10 PQC APIs.

## Example 1: Secure Message Exchange with ML-KEM

Demonstrates how two parties can securely establish a shared secret for encrypting messages.

```csharp
using System.Security.Cryptography;
using System.Text;

public class SecureMessageExchange
{
    // Simulate Alice sending a secure message to Bob
    public static void Example()
    {
        // Bob generates a key pair and shares his public key
        using (MLKem bob_KeyPair = MLKem.GenerateKey(MLKemAlgorithm.MLKem768))
        {
            string bob_PublicKeyPem = bob_KeyPair.ExportSubjectPublicKeyInfoPem();
            string bob_PrivateKeyPem = bob_KeyPair.ExportPkcs8PrivateKeyPem();

            // Alice imports Bob's public key
            using (MLKem alice_PublicKey = MLKem.ImportFromPem(bob_PublicKeyPem))
            {
                // Alice generates shared secret and ciphertext
                alice_PublicKey.Encapsulate(out byte[] ciphertext, out byte[] shared_secret);

                Console.WriteLine($"Alice creates ciphertext: {Convert.ToHexString(ciphertext[..16])}...");
                Console.WriteLine($"Shared secret (for AES): {Convert.ToHexString(shared_secret[..16])}...");

                // Alice encrypts message with shared secret (using AES)
                byte[] message = Encoding.UTF8.GetBytes("Secret message from Alice");
                byte[] encrypted = EncryptWithAES(message, shared_secret);

                // Alice sends ciphertext and encrypted message to Bob

                // Bob receives ciphertext and encrypted message
                // Bob imports his private key
                using (MLKem bob_PrivateKey = MLKem.ImportFromPem(bob_PrivateKeyPem))
                {
                    // Bob recovers shared secret with his private key
                    byte[] bob_shared_secret = new byte[shared_secret.Length];
                    bob_PrivateKey.Decapsulate(ciphertext, bob_shared_secret);

                    Console.WriteLine($"Bob recovers secret: {Convert.ToHexString(bob_shared_secret[..16])}...");

                    // Bob decrypts message with recovered secret
                    byte[] decrypted = DecryptWithAES(encrypted, bob_shared_secret);
                    string message_text = Encoding.UTF8.GetString(decrypted);

                    Console.WriteLine($"Bob decrypts: {message_text}");
                }
            }
        }
    }

    private static byte[] EncryptWithAES(byte[] plaintext, byte[] key)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = key[..32]; // Use first 32 bytes for AES-256
            aes.GenerateIV();

            using (var encryptor = aes.CreateEncryptor())
            using (var ms = new MemoryStream())
            {
                ms.Write(aes.IV, 0, aes.IV.Length);
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(plaintext, 0, plaintext.Length);
                }
                return ms.ToArray();
            }
        }
    }

    private static byte[] DecryptWithAES(byte[] ciphertext, byte[] key)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = key[..32]; // Use first 32 bytes for AES-256
            aes.IV = ciphertext[..aes.IV.Length];

            using (var decryptor = aes.CreateDecryptor())
            using (var ms = new MemoryStream(ciphertext, aes.IV.Length, ciphertext.Length - aes.IV.Length))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var result = new MemoryStream())
            {
                cs.CopyTo(result);
                return result.ToArray();
            }
        }
    }
}
```

## Example 2: Document Signing with ML-DSA

Demonstrates how to sign documents and verify signatures using ML-DSA.

```csharp
using System.Security.Cryptography;
using System.Text;

public class DocumentSignature
{
    public class SignedDocument
    {
        public string Content { get; set; }
        public byte[] Signature { get; set; }
        public string PublicKeyPem { get; set; }
        public DateTime SignedAt { get; set; }
    }

    public static void Example()
    {
        // Developer signs document
        byte[] documentContent = Encoding.UTF8.GetBytes("Important contract document");

        using (MLDsa signingKey = MLDsa.GenerateKey(MLDsaAlgorithm.MLDsa65))
        {
            byte[] signature = signingKey.SignData(documentContent);
            string publicKeyPem = signingKey.ExportSubjectPublicKeyInfoPem();

            SignedDocument doc = new SignedDocument
            {
                Content = Encoding.UTF8.GetString(documentContent),
                Signature = signature,
                PublicKeyPem = publicKeyPem,
                SignedAt = DateTime.UtcNow
            };

            Console.WriteLine($"Document signed at {doc.SignedAt:O}");
            Console.WriteLine($"Signature (first 32 bytes): {Convert.ToHexString(signature[..32])}...");

            // Recipient verifies document
            VerifyDocument(doc);
        }
    }

    public static bool VerifyDocument(SignedDocument doc)
    {
        using (MLDsa verifyingKey = MLDsa.ImportFromPem(doc.PublicKeyPem))
        {
            byte[] documentContent = Encoding.UTF8.GetBytes(doc.Content);
            bool isValid = verifyingKey.VerifyData(documentContent, doc.Signature);

            if (isValid)
            {
                Console.WriteLine("✓ Document verified - signature is authentic");
            }
            else
            {
                Console.WriteLine("✗ Document verification failed - signature is invalid");
            }

            return isValid;
        }
    }
}
```

## Example 3: Hybrid Cryptography (Traditional + PQC)

Demonstrates combining traditional and PQC algorithms for maximum security during transition.

```csharp
using System.Security.Cryptography;

public class HybridCryptography
{
    public static void Example()
    {
        // Generate both traditional and PQC keys
        using (ECDsa ecdsaKey = ECDsa.Create(ECCurve.NamedCurves.nistP384))
        using (MLDsa mldsaKey = MLDsa.GenerateKey(MLDsaAlgorithm.MLDsa65))
        {
            byte[] message = new byte[] { 1, 2, 3, 4, 5 };

            // Sign with both algorithms
            byte[] ecdsaSignature = ecdsaKey.SignData(message, HashAlgorithmName.SHA384);
            byte[] mldsaSignature = mldsaKey.SignData(message);

            Console.WriteLine($"ECDSA signature: {ecdsaSignature.Length} bytes");
            Console.WriteLine($"ML-DSA signature: {mldsaSignature.Length} bytes");

            // Verify with both
            bool ecdsaValid = ecdsaKey.VerifyData(message, ecdsaSignature, HashAlgorithmName.SHA384);
            bool mldsaValid = mldsaKey.VerifyData(message, mldsaSignature);

            bool allValid = ecdsaValid && mldsaValid;
            Console.WriteLine($"Hybrid verification: {(allValid ? "PASS" : "FAIL")}");

            // For certificates, the hybrid approach ensures backward compatibility
            // while being quantum-safe
        }
    }
}
```

## Example 4: Key Rotation Strategy

Shows how to implement key rotation with new PQC systems.

```csharp
using System.Security.Cryptography;

public class KeyRotation
{
    public class KeySet
    {
        public string PublicKeyPem { get; set; }
        public string PrivateKeyPem { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Algorithm { get; set; }
    }

    public static void Example()
    {
        var keyRotationSchedule = new List<KeySet>();

        // Year 1: Hybrid (ECDSA + ML-DSA)
        var year1Keys = GenerateHybridKeySet();
        keyRotationSchedule.Add(year1Keys);

        // Year 2: Enhanced ML-DSA (larger variant)
        var year2Keys = GenerateMLDsaKeySet(MLDsaAlgorithm.MLDsa65);
        keyRotationSchedule.Add(year2Keys);

        // Year 3+: Pure PQC (as quantum threat becomes clearer)
        var year3Keys = GenerateMLDsaKeySet(MLDsaAlgorithm.MLDsa87);
        keyRotationSchedule.Add(year3Keys);

        // Display rotation schedule
        foreach (var keySet in keyRotationSchedule)
        {
            Console.WriteLine($"{keySet.Algorithm}");
            Console.WriteLine($"  Created: {keySet.CreatedAt:yyyy-MM-dd}");
            Console.WriteLine($"  Expires: {keySet.ExpiresAt:yyyy-MM-dd}");
        }
    }

    private static KeySet GenerateHybridKeySet()
    {
        using (var mlDsa = MLDsa.GenerateKey(MLDsaAlgorithm.MLDsa65))
        {
            return new KeySet
            {
                PublicKeyPem = mlDsa.ExportSubjectPublicKeyInfoPem(),
                PrivateKeyPem = mlDsa.ExportPkcs8PrivateKeyPem(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddYears(1),
                Algorithm = "Hybrid (ECDSA + ML-DSA-65)"
            };
        }
    }

    private static KeySet GenerateMLDsaKeySet(MLDsaAlgorithm algorithm)
    {
        using (var mlDsa = MLDsa.GenerateKey(algorithm))
        {
            return new KeySet
            {
                PublicKeyPem = mlDsa.ExportSubjectPublicKeyInfoPem(),
                PrivateKeyPem = mlDsa.ExportPkcs8PrivateKeyPem(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddYears(1),
                Algorithm = $"ML-DSA ({algorithm})"
            };
        }
    }
}
```

## Example 5: Web Service Scenario

Demonstrates using PQC for a REST API authentication mechanism.

```csharp
using System.Security.Cryptography;
using System.Text.Json;
using System.Text;

public class PQCWebService
{
    public class AuthToken
    {
        public string UserId { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Nonce { get; set; }
        public byte[] Signature { get; set; }
    }

    private readonly MLDsa _serviceSigningKey;
    private readonly string _servicePublicKeyPem;

    public PQCWebService()
    {
        // Service generates its key pair once at startup
        _serviceSigningKey = MLDsa.GenerateKey(MLDsaAlgorithm.MLDsa65);
        _servicePublicKeyPem = _serviceSigningKey.ExportSubjectPublicKeyInfoPem();
    }

    // Client authenticates with the service
    public AuthToken AuthenticateClient(string userId, string password)
    {
        // In production: validate userId and password securely

        var tokenData = new
        {
            userId,
            issuedAt = DateTime.UtcNow,
            expiresAt = DateTime.UtcNow.AddHours(1),
            nonce = Guid.NewGuid().ToString()
        };

        string json = JsonSerializer.Serialize(tokenData);
        byte[] tokenBytes = Encoding.UTF8.GetBytes(json);

        // Service signs the token with its private key
        byte[] signature = _serviceSigningKey.SignData(tokenBytes);

        return new AuthToken
        {
            UserId = userId,
            IssuedAt = tokenData.issuedAt,
            ExpiresAt = tokenData.expiresAt,
            Nonce = tokenData.nonce,
            Signature = signature
        };
    }

    // Client verifies the token
    public bool VerifyToken(AuthToken token)
    {
        var tokenData = new
        {
            userId = token.UserId,
            issuedAt = token.IssuedAt,
            expiresAt = token.ExpiresAt,
            nonce = token.Nonce
        };

        string json = JsonSerializer.Serialize(tokenData);
        byte[] tokenBytes = Encoding.UTF8.GetBytes(json);

        // Verify signature with service's public key
        using (MLDsa verifyingKey = MLDsa.ImportFromPem(_servicePublicKeyPem))
        {
            bool isValid = verifyingKey.VerifyData(tokenBytes, token.Signature);

            // Also check expiration
            bool notExpired = DateTime.UtcNow < token.ExpiresAt;

            return isValid && notExpired;
        }
    }

    public void Cleanup()
    {
        _serviceSigningKey?.Dispose();
    }
}
```

## Running the Examples

To use these examples in your own projects:

1. Add appropriate error handling
2. Implement proper key storage/retrieval
3. Add logging and monitoring
4. Implement certificate management
5. Consider compliance requirements (FIPS, etc.)
6. Test thoroughly before production deployment

## Common Pitfalls to Avoid

1. **Not checking IsSupported**: Always verify PQC is available on the platform
2. **Mixing key types**: Don't try to use ML-KEM keys with ML-DSA operations
3. **Reusing shared secrets**: Generate new shared secret for each session
4. **Ignoring signature size**: Account for larger signatures in bandwidth-constrained scenarios
5. **Not implementing key rotation**: Plan for algorithm updates
6. **Forgetting to dispose**: Always use `using` statements for disposable crypto objects
7. **No tampering detection**: Always verify signatures before processing data
