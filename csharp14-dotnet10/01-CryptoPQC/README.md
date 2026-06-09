# Post-Quantum Cryptography (PQC) Sample for .NET 10

This sample demonstrates the new Post-Quantum Cryptography (PQC) support in .NET 10, showcasing how to use quantum-resistant algorithms that are standardized by NIST.

## Overview

Post-Quantum Cryptography addresses the threat posed by quantum computers to classical asymmetric cryptography. While quantum computers don't pose an immediate threat, organizations should begin transitioning to quantum-resistant algorithms now to protect against "harvest now, decrypt later" attacks.

## What's Changed from Legacy Cryptography

### Traditional Cryptography (Legacy)
- **RSA**: Based on integer factorization problem
- **ECDSA**: Based on elliptic curve discrete logarithm problem
- **ECDH**: Key exchange using elliptic curves
- **Thread**: Quantum computers could solve these in polynomial time
- **Key Sizes**: Relatively small (256-2048 bits)

### Post-Quantum Cryptography (New)
- **ML-KEM** (FIPS 203): Based on lattice-based math (Module-Lattice-Based Key-Encapsulation Mechanism)
- **ML-DSA** (FIPS 204): Based on lattice-based math (Module-Lattice-Based Digital Signature Algorithm)
- **Resistance**: Resistant to both classical and quantum attacks
- **Key Sizes**: Larger (1000+ bytes)
- **Performance**: Comparable to or faster than RSA/ECDSA

## Key Features Demonstrated

### 1. ML-KEM (Key Encapsulation)
ML-KEM is used for establishing shared secrets between two parties:
```csharp
// Generate key pair
using (MLKem keyPair = MLKem.GenerateKey(MLKemAlgorithm.MLKem768))
{
    // Encapsulation: Create shared secret and ciphertext
    keyPair.Encapsulate(out byte[] ciphertext, out byte[] sharedSecret);

    // Decapsulation: Recover shared secret
    using (MLKem decapsulator = MLKem.ImportFromPem(privateKeyPem))
    {
        byte[] recoveredSecret = new byte[sharedSecret.Length];
        decapsulator.Decapsulate(ciphertext, recoveredSecret);
    }
}
```

**Use Cases:**
- TLS 1.3 key exchange (post-quantum safe)
- VPN protocols
- Hybrid approaches with traditional ECDH

**Advantages:**
- Deterministic shared secret generation
- Same secret for both parties from ciphertext
- Perfect forward secrecy with proper implementation

### 2. ML-DSA (Digital Signatures)
ML-DSA provides quantum-resistant digital signatures:
```csharp
// Generate key pair
using (MLDsa signingKey = MLDsa.GenerateKey(MLDsaAlgorithm.MLDsa65))
{
    // Sign data
    byte[] signature = signingKey.SignData(message);

    // Verify signature
    using (MLDsa verifyingKey = MLDsa.ImportFromPem(publicKeyPem))
    {
        bool isValid = verifyingKey.VerifyData(message, signature);
    }
}
```

**Use Cases:**
- Code signing
- X.509 certificate generation
- Authentication and non-repudiation
- Document signing

**Advantages:**
- Deterministic signatures
- Larger signatures but more secure
- NIST standardized

## Algorithm Variants

### ML-KEM Variants
- **ML-KEM-512**: NIST Security Level 1 (smallest, fastest)
- **ML-KEM-768**: NIST Security Level 3 (recommended)
- **ML-KEM-1024**: NIST Security Level 5 (strongest)

### ML-DSA Variants
- **ML-DSA-44**: NIST Security Level 2
- **ML-DSA-65**: NIST Security Level 3 (recommended)
- **ML-DSA-87**: NIST Security Level 5

## Platform Support

| Platform | Support | Requirements |
|----------|---------|--------------|
| Windows  | ✓ | Windows 11 Insiders with latest CNG |
| Linux    | ✓ | OpenSSL 3.5.0 or newer |
| macOS    | ✗ | Not supported yet |

You can check support at runtime:
```csharp
if (MLKem.IsSupported && MLDsa.IsSupported)
{
    // PQC algorithms are available
}
```

## Size Comparison

### Key Sizes
| Algorithm    | Public Key  | Private Key |
|--------------|-------------|-------------|
| RSA-2048     | 294 bytes   | 1704 bytes  |
| ECDSA-P256   | 91 bytes    | 139 bytes   |
| ML-KEM-768   | 1,184 bytes | 2,400 bytes |
| ML-DSA-65    | 1,952 bytes | 4,032 bytes |

### Artifact Sizes
| Algorithm    | Artifact Size | Type        |
|--------------|--------------|-------------|
| RSA-2048     | 256 bytes    | Signature   |
| ECDSA-P256   | 64-72 bytes  | Signature   |
| ML-KEM-768   | 1,088 bytes  | Ciphertext  |
| ML-DSA-65    | 3,309 bytes  | Signature   |

## Advantages of PQC

1. **Quantum Resistance**: Safe against future quantum computers
2. **NIST Standardized**: Vetted by security experts
3. **Built-in Support**: No external dependencies in .NET 10
4. **Compatible**: Can work with existing cryptographic infrastructure
5. **Performance**: Comparable to or better than RSA for many operations

## Trade-offs

1. **Larger Keys/Signatures**: Requires more storage and bandwidth
2. **Newer Standard**: Less deployment history than RSA/ECDSA
3. **Experimental APIs**: Currently marked as experimental (SYSLIB5006)
4. **Platform Limited**: Not all platforms supported yet

## Migration Path

For existing applications:
1. **Hybrid Approach**: Use both traditional (RSA/ECDSA) and PQC algorithms
2. **Gradual Transition**: Start with non-critical systems
3. **Certificate Management**: PQC certificates for new deployments
4. **Backward Compatibility**: Support both old and new algorithms

## Running the Sample

### Prerequisites
- .NET 10 SDK installed
- Windows 11 with latest updates or Linux with OpenSSL 3.5+

### Execution
```bash
cd 2026-Copilot/CryptoPQC/CryptoPQC
dotnet run
```

### Expected Output
The sample will:
1. Check platform support for PQC
2. Demonstrate ML-KEM key agreement
3. Demonstrate ML-DSA digital signatures
4. Show performance benchmarks
5. Compare key and signature sizes

## References

- [NIST Post-Quantum Cryptography](https://csrc.nist.gov/projects/post-quantum-cryptography/)
- [ML-KEM FIPS 203](https://csrc.nist.gov/pubs/fips/203/final)
- [ML-DSA FIPS 204](https://csrc.nist.gov/pubs/fips/204/final)
- [.NET 10 Cryptography Features](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-10/libraries#cryptography)
- [Microsoft's Quantum-Resistant Cryptography](https://www.microsoft.com/security/blog/2022/08/04/microsoft-is-delivering-post-quantum-cryptography-to-protect-hybrid-environments/)

## Notes

- These APIs are experimental and marked with `[Experimental]`
- The sample uses `#pragma warning disable SYSLIB5006` to suppress experimental API warnings
- In production, carefully manage the deprecation warnings and ensure migration before finalization
- Hybrid approaches are recommended during the transition period
