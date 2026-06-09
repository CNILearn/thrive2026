# Post-Quantum Cryptography API Comparison

## Traditional Cryptography APIs vs PQC APIs

### Key Generation Pattern

#### Traditional (RSA/ECDSA)
```csharp
using (RSA rsa = RSA.Create(2048))
{
    // Key is generated in constructor
}

using (ECDsa ecdsa = ECDsa.Create())
{
    // Key is generated in constructor
}
```

#### Post-Quantum (ML-KEM/ML-DSA)
```csharp
using (MLKem kem = MLKem.GenerateKey(MLKemAlgorithm.MLKem768))
{
    // Generated with static method
}

using (MLDsa dsa = MLDsa.GenerateKey(MLDsaAlgorithm.MLDsa65))
{
    // Generated with static method
}
```

**Key Difference**: PQC uses explicit `GenerateKey()` method instead of relying on constructors.

### Export/Import Pattern

#### Traditional Asymmetric Algorithm Pattern
```csharp
// Export
byte[] publicKey = rsa.ExportSubjectPublicKeyInfo();
byte[] privateKey = rsa.ExportPkcs8PrivateKey();

// Import
using (RSA importedRsa = RSA.Create())
{
    importedRsa.ImportSubjectPublicKeyInfo(publicKey, out _);
}
```

#### Post-Quantum Pattern
```csharp
// Export (same pattern)
byte[] publicKeyInfo = mlkem.ExportSubjectPublicKeyInfo();
string publicKeyPem = mlkem.ExportSubjectPublicKeyInfoPem();
string privateKeyPem = mlkem.ExportPkcs8PrivateKeyPem();

// Import (different - static method)
using (MLKem imported = MLKem.ImportFromPem(publicKeyPem))
{
    // Directly imported
}
```

**Key Difference**: PQC uses static import methods, not instance methods.

### Signature Operations

#### Traditional (RSA/ECDSA)
```csharp
// Sign
byte[] signature = rsa.SignData(message, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

// Verify
bool valid = rsa.VerifyData(message, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
```

#### Post-Quantum (ML-DSA)
```csharp
// Sign - no hash algorithm needed (built-in)
byte[] signature = mlDsa.SignData(message);

// Verify - simpler API
bool valid = mlDsa.VerifyData(message, signature);
```

**Key Difference**: PQC signatures are simpler; hashing is built-in to the algorithm.

### Key Encapsulation Pattern

#### Traditional - No Direct Equivalent
```csharp
// Closest is ECDH, but uses a different pattern
using (ECDiffieHellman ecdh1 = ECDiffieHellman.Create())
using (ECDiffieHellman ecdh2 = ECDiffieHellman.Create())
{
    byte[] shared = ecdh1.DeriveKeyMaterial(ecdh2.PublicKey);
}
```

#### Post-Quantum (ML-KEM) - Purpose-Built
```csharp
// Encapsulation
mlkem.Encapsulate(out byte[] ciphertext, out byte[] sharedSecret);

// Decapsulation
mlkemPrivate.Decapsulate(ciphertext, recoveredSecret);
```

**Key Difference**: ML-KEM has dedicated encapsulation/decapsulation, not key derivation.

## Detailed API Examples

### ML-KEM Example

```csharp
// Generate keypair
using (MLKem keyPair = MLKem.GenerateKey(MLKemAlgorithm.MLKem768))
{
    // Export keys for transmission
    string publicKeyPem = keyPair.ExportSubjectPublicKeyInfoPem();
    byte[] publicKeyDer = keyPair.ExportSubjectPublicKeyInfo();

    // Optional: Get the encapsulation key (smaller than full public key)
    byte[] encapsulationKey = keyPair.ExportEncapsulationKey();

    // Encapsulation (sender side)
    keyPair.Encapsulate(out byte[] ciphertext, out byte[] sharedSecret);

    // Send ciphertext to recipient
    Console.WriteLine($"Send ciphertext: {Convert.ToHexString(ciphertext)}");

    // Recipient imports private key and decapsulates
    string privateKeyPem = keyPair.ExportPkcs8PrivateKeyPem();
    using (MLKem recipient = MLKem.ImportFromPem(privateKeyPem))
    {
        byte[] recoveredSecret = new byte[sharedSecret.Length];
        recipient.Decapsulate(ciphertext, recoveredSecret);

        // Both parties now have same shared secret
        bool match = sharedSecret.SequenceEqual(recoveredSecret);
        System.Diagnostics.Debug.Assert(match);
    }
}
```

### ML-DSA Example

```csharp
// Generate keypair
using (MLDsa signingKey = MLDsa.GenerateKey(MLDsaAlgorithm.MLDsa65))
{
    // Export keys
    string publicKeyPem = signingKey.ExportSubjectPublicKeyInfoPem();
    string privateKeyPem = signingKey.ExportPkcs8PrivateKeyPem();

    byte[] message = Encoding.UTF8.GetBytes("Important message");

    // Sign
    byte[] signature = signingKey.SignData(message);

    // Verify (with same key first)
    bool valid1 = signingKey.VerifyData(message, signature);

    // Verify with public key (as recipient would)
    using (MLDsa verifyingKey = MLDsa.ImportFromPem(publicKeyPem))
    {
        bool valid2 = verifyingKey.VerifyData(message, signature);
        System.Diagnostics.Debug.Assert(valid1 && valid2);
    }

    // Tampering detection
    byte[] tamperedMessage = Encoding.UTF8.GetBytes("Tampered message");
    bool valid3 = signingKey.VerifyData(tamperedMessage, signature);  // false

    System.Diagnostics.Debug.Assert(!valid3, "Tampering should be detected");
}
```

## Performance Characteristics

### ML-KEM Performance
- **Encapsulation**: ~1-2µs (microseconds)
- **Decapsulation**: ~5-10µs (microseconds)
- **Key Generation**: ~100-200µs (microseconds)

### ML-DSA Performance
- **Signing**: ~50-100µs (microseconds)
- **Verification**: ~100-200µs (microseconds)
- **Key Generation**: ~1-5ms (milliseconds)

### Comparison with Traditional
- **RSA-2048 Signing**: ~1-5ms
- **ECDSA-P256 Signing**: ~100-500µs
- **RSA-2048 Verification**: ~0.1-0.5ms
- **ECDSA-P256 Verification**: ~100-500µs

**Summary**: ML-DSA is slower than ECDSA but comparable to RSA for signing; verification is faster than both.

## Security Levels

### NIST Security Levels
| Level | Description | Comparable to |
|-------|-------------|--------------|
| Level 1 | Basic security | AES-128 |
| Level 2 | Enhanced security | RSA-2048 |
| Level 3 | High security (recommended) | RSA-3072, SHA-256 |
| Level 4 | Very high security | RSA-7680 |
| Level 5 | Maximum security | AES-256 |

### Recommended Choices
- **ML-KEM-768** (Level 3) - For general use
- **ML-DSA-65** (Level 3) - For signatures
- **Larger variants** for government/military applications

## Migration Checklist

For transitioning from traditional to PQC:

- [ ] Audit current asymmetric cryptography usage
- [ ] Identify dependencies on specific key sizes
- [ ] Evaluate hybrid approaches (traditional + PQC)
- [ ] Update certificate management systems
- [ ] Test performance impact with new key sizes
- [ ] Plan key rotation strategy
- [ ] Update documentation and APIs
- [ ] Implement gradual rollout
- [ ] Monitor for compatibility issues
- [ ] Plan for future algorithm updates

## Further Reading

- [ML-KEM Specification](https://csrc.nist.gov/pubs/fips/203/final)
- [ML-DSA Specification](https://csrc.nist.gov/pubs/fips/204/final)
- [.NET API Documentation](https://learn.microsoft.com/dotnet/api/system.security.cryptography)
- [Post-Quantum Cryptography Primer](https://www.cloudflare.com/learning/ssl/post-quantum-cryptography/)
