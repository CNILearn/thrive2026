using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace CryptoPQC;

internal class PQCUtilities
{
    // =====================================================================
    // Post-Quantum Key Encapsulation Mechanism (ML-KEM - FIPS 203)
    // =====================================================================
    public static async Task DemonstratePQCKeyEncapsulation()
    {
        Console.WriteLine(
            """
        ╔════════════════════════════════════════════════════════════╗
        ║ 1. ML-KEM (Module-Lattice-Based KEM) - FIPS 203          ║
        ║    Key Encapsulation Mechanism for secure key agreement  ║
        ╚════════════════════════════════════════════════════════════╝

        """);

        Console.WriteLine(
            """
        📝 Generating ML-KEM-768 key pair...
           (NIST Security Level 3, recommended for most uses)

        """);

        using (MLKem keyPair = MLKem.GenerateKey(MLKemAlgorithm.MLKem768))
        {
            string publicKeyPem = keyPair.ExportSubjectPublicKeyInfoPem();
            string privateKeyPem = keyPair.ExportPkcs8PrivateKeyPem();

            // Get encapsulation key size
            byte[] encapsulationKey = keyPair.ExportEncapsulationKey();

            Console.WriteLine($"   ✓ Public key: {publicKeyPem.Split('\n')[0]}...");
            Console.WriteLine($"   ✓ Encapsulation key size: {encapsulationKey.Length} bytes");

            // Encapsulation: Generate shared secret and ciphertext
            Console.WriteLine(
                """

            🔐 Encapsulation Phase (simulating Alice sending to Bob):
               Alice has Bob's public key and creates a shared secret...

            """);

            keyPair.Encapsulate(out byte[] ciphertext, out byte[] sharedSecret);

            Console.WriteLine($"   ✓ Shared secret: {sharedSecret.Length} bytes");
            Console.WriteLine($"   ✓ Encapsulated ciphertext: {ciphertext.Length} bytes");
            Console.WriteLine($"   ✓ Ciphertext hex (first 32 bytes): {Convert.ToHexString(ciphertext[..32])}...");

            // Decapsulation: Recover the shared secret using private key
            Console.WriteLine(
                """

            🔓 Decapsulation Phase (simulating Bob receiving):
               Bob receives the ciphertext and uses private key...

            """);

            // For demo purposes, we create a new key pair for Bob
            using MLKem bobKeyPair = MLKem.GenerateKey(MLKemAlgorithm.MLKem768);
            // Simulate key exchange with Bob's keys
            bobKeyPair.Encapsulate(out byte[] bobCiphertext, out byte[] bobSharedSecret);

            // Import Bob's private key and decapsulate
            string bobPrivateKeyPem = bobKeyPair.ExportPkcs8PrivateKeyPem();
            using MLKem decapsulator = MLKem.ImportFromPem(bobPrivateKeyPem);
            byte[] recoveredSecret = new byte[bobSharedSecret.Length];
            decapsulator.Decapsulate(bobCiphertext, recoveredSecret);

            Console.WriteLine($"   ✓ Recovered secret size: {recoveredSecret.Length} bytes");
            bool secretsMatch = bobSharedSecret.SequenceEqual(recoveredSecret);
            Console.WriteLine($"   ✓ Secrets match (deterministic): {secretsMatch}");

            if (secretsMatch)
            {
                Console.WriteLine(
                    """

                ✅ ML-KEM Shared Secret Agreement successful!
                   Both parties now have the same shared secret which can be used
                   with a symmetric cipher like AES to encrypt/decrypt communication.
                """);
            }
        }

        Console.WriteLine(
            """

        📊 ML-KEM Use Cases:
           • Perfect for TLS 1.3 key exchange (post-quantum safe)
           • VPN and secure communications protocols
           • Hybrid approaches with traditional ECDH
           • Replaces: ECDH, RSA-KEM for key agreement

        """);
    }

    // =====================================================================
    // Post-Quantum Digital Signatures (ML-DSA - FIPS 204)
    // =====================================================================
    public static async Task DemonstratePQCSignatures()
    {
        Console.WriteLine(
            """
        ╔════════════════════════════════════════════════════════════╗
        ║ 2. ML-DSA (Module-Lattice-Based DSA) - FIPS 204          ║
        ║    Digital Signature Algorithm for authentication        ║
        ╚════════════════════════════════════════════════════════════╝

        """);

        Console.WriteLine(
            """
        📝 Generating ML-DSA-65 key pair...
           (NIST Security Level 3, recommended for most uses)

        """);

        using MLDsa signingKey = MLDsa.GenerateKey(MLDsaAlgorithm.MLDsa65);

        string publicKeyPem = signingKey.ExportSubjectPublicKeyInfoPem();
        string privateKeyPem = signingKey.ExportPkcs8PrivateKeyPem();

        Console.WriteLine($"   ✓ Public key: {publicKeyPem.Split('\n')[0]}...");
        Console.WriteLine($"   ✓ Private key: {privateKeyPem.Split('\n')[0]}...");

        // Message to sign
        byte[] message = Encoding.UTF8.GetBytes("This is a quantum-resistant digital signature!");
        Console.WriteLine($"\n📄 Message to sign: \"{Encoding.UTF8.GetString(message)}\"");

        // Sign the message
        Console.WriteLine("\n✍️  Signing message with private key...");
        byte[] signature = signingKey.SignData(message);
        Console.WriteLine($"   ✓ Signature size: {signature.Length} bytes");
        Console.WriteLine($"   ✓ Signature hex (first 32 bytes): {Convert.ToHexString(signature[..32])}...");

        // Verify the signature
        Console.WriteLine("\n✔️  Verifying signature with public key...");
        using MLDsa verifyingKey = MLDsa.ImportFromPem(publicKeyPem);
        bool isValid = verifyingKey.VerifyData(message, signature);
        Console.WriteLine($"   ✓ Signature valid: {isValid}");

        if (isValid)
        {
            Console.WriteLine(
                """

                ✅ ML-DSA Digital Signature verified successfully!
                   The message authenticity and integrity are confirmed.
                """);
        }

        // Demonstrate tampering detection
        Console.WriteLine("\n🔨 Demonstrating tampering detection...");
        Console.WriteLine("   Simulating message modification...");

        byte[] tamperedMessage = Encoding.UTF8.GetBytes("This is a TAMPERED message!");
        byte[] tamperedSignature = new byte[signature.Length];
        Array.Copy(signature, tamperedSignature, signature.Length);
        tamperedSignature[0] ^= 0xFF; // Flip bits in signature

        bool tamperedIsValid = verifyingKey.VerifyData(tamperedMessage, tamperedSignature);
        Console.WriteLine($"   ✓ Tampered message valid: {tamperedIsValid}");
        if (!tamperedIsValid)
        {
            Console.WriteLine("   ✓ Tampering detected successfully! ✓");
        }


        Console.WriteLine(
            """

        📊 ML-DSA Use Cases:
           • Digital code signing
           • X.509 certificate generation
           • Authentication and non-repudiation
           • Document signing
           • Replaces: RSA signatures, ECDSA

        """);
    }

    //
    // =====================================================================
    // Performance Comparison: Legacy vs PQC
    // =====================================================================
    public static async Task ComparePerformance()
    {
        const int iterations = 1000;  // More iterations for better accuracy
        byte[] message = Encoding.UTF8.GetBytes("Benchmark message for cryptographic operations");

        Console.WriteLine("Benchmarking performance (note: results vary by system and implementation):\n");

        // ===== LEGACY ALGORITHMS =====
        Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║ LEGACY ALGORITHMS (Quantum-Vulnerable)                   ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

        // RSA-2048: Key Gen vs Operations
        Console.WriteLine("RSA-2048 (Legacy Key Exchange)");
        Console.WriteLine("  ┌─ Key Generation:");
        using (var rsa = RSA.Create(2048))
        {
            var keyGenWatch = Stopwatch.StartNew();
            using var rsaKeyGen = RSA.Create(2048);
            keyGenWatch.Stop();
            Console.WriteLine($"     └─ One-time cost: {keyGenWatch.ElapsedMilliseconds}ms");

            Console.WriteLine("  ┌─ Encryption Operations:");
            var opsWatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                byte[] encryptedData = rsa.Encrypt(message, RSAEncryptionPadding.OaepSHA256);
            }
            opsWatch.Stop();
            Console.WriteLine($"     ├─ {iterations} ops: {opsWatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"     └─ Avg per op: {opsWatch.ElapsedMilliseconds / (double)iterations:F3}ms\n");
        }

        // ECDSA-P256: Key Gen vs Operations
        Console.WriteLine("ECDSA-P256 (Legacy Signing)");
        Console.WriteLine("  ┌─ Key Generation:");
        using (var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256))
        {
            var keyGenWatch = Stopwatch.StartNew();
            using var ecdsaKeyGen = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            keyGenWatch.Stop();
            Console.WriteLine($"     └─ One-time cost: {keyGenWatch.ElapsedMilliseconds}ms");

            Console.WriteLine("  ┌─ Signing Operations:");
            var opsWatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                byte[] signature = ecdsa.SignData(message, HashAlgorithmName.SHA256);
            }
            opsWatch.Stop();
            Console.WriteLine($"     ├─ {iterations} ops: {opsWatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"     └─ Avg per op: {opsWatch.ElapsedMilliseconds / (double)iterations:F3}ms\n");
        }

        // ===== PQC ALGORITHMS =====
        Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║ POST-QUANTUM ALGORITHMS (Quantum-Safe)                   ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

        // ML-KEM-768: Key Gen vs Operations
        Console.WriteLine("ML-KEM-768 (PQC Key Encapsulation)");
        Console.WriteLine("  ┌─ Key Generation:");
        var kemKeyGenWatch = Stopwatch.StartNew();
        using (MLKem kemKeyPair = MLKem.GenerateKey(MLKemAlgorithm.MLKem768))
        {
            kemKeyGenWatch.Stop();
            Console.WriteLine($"     └─ One-time cost: {kemKeyGenWatch.ElapsedMilliseconds}ms");

            Console.WriteLine("  ┌─ Encapsulation Operations:");
            var opsWatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                kemKeyPair.Encapsulate(out byte[] ct, out byte[] ss);
            }
            opsWatch.Stop();
            Console.WriteLine($"     ├─ {iterations} ops: {opsWatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"     └─ Avg per op: {opsWatch.ElapsedMilliseconds / (double)iterations:F3}ms\n");
        }

        // ML-DSA-65: Key Gen vs Operations
        Console.WriteLine("ML-DSA-65 (PQC Digital Signatures)");
        Console.WriteLine("  ┌─ Key Generation:");
        var dsaKeyGenWatch = Stopwatch.StartNew();
        using (MLDsa dsaKeyPair = MLDsa.GenerateKey(MLDsaAlgorithm.MLDsa65))
        {
            dsaKeyGenWatch.Stop();
            Console.WriteLine($"     └─ One-time cost: {dsaKeyGenWatch.ElapsedMilliseconds}ms");

            Console.WriteLine("  ┌─ Signing Operations:");
            var opsWatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                byte[] signature = dsaKeyPair.SignData(message);
            }
            opsWatch.Stop();
            Console.WriteLine($"     ├─ {iterations} ops: {opsWatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"     └─ Avg per op: {opsWatch.ElapsedMilliseconds / (double)iterations:F3}ms\n");
        }

        // Size and Performance Comparison - Legacy vs PQC Algorithms
        Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║ CRYPTOGRAPHIC ARTIFACT SIZES                             ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

        Console.WriteLine(
            """
          Algorithm           │ Type           │ Key Size       │ Artifact Size  │ Quantum-Safe
          ────────────────────────────────────────────────────────────────────────────────────
        """);

        using (RSA rsa = RSA.Create(2048))
        {
            byte[] rsaSig = rsa.SignData(message, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            Console.WriteLine($"  RSA-2048            │ Legacy          │ 2048 bits      │ 256 bytes      │ ✗ Vulnerable");
        }

        using (ECDsa ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256))
        {
            byte[] ecdsaSig = ecdsa.SignData(message, HashAlgorithmName.SHA256);
            Console.WriteLine($"  ECDSA-P256          │ Legacy          │ 256 bits       │ 64-72 bytes    │ ✗ Vulnerable");
        }

        using (MLKem mlkem = MLKem.GenerateKey(MLKemAlgorithm.MLKem768))
        {
            byte[] eKey = mlkem.ExportEncapsulationKey();
            mlkem.Encapsulate(out byte[] ct, out byte[] ss);
            Console.WriteLine($"  ML-KEM-768          │ PQC (FIPS 203)  │ 1184 bytes     │ 1088 bytes     │ ✓ Protected");
        }

        using (MLDsa mldsa = MLDsa.GenerateKey(MLDsaAlgorithm.MLDsa65))
        {
            byte[] sig = mldsa.SignData(message);
            Console.WriteLine($"  ML-DSA-65           │ PQC (FIPS 204)  │ 1952 bytes     │ 3309 bytes     │ ✓ Protected");
        }

        Console.WriteLine();
        Console.WriteLine(
            """
        📊 Key Takeaways:
           • RSA key generation is VERY expensive (why it appears slow)
           • PQC operations are more complex but highly optimized in .NET
           • For repeated operations with reused keys: measure operations only
           • For TLS handshakes: key generation happens once at startup
           • The real advantage: PQC resists quantum computing attacks

        """);
    }


}
