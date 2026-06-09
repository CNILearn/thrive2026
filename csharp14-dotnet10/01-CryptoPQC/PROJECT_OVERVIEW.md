# CryptoPQC Sample Project - Complete Overview

This project demonstrates Post-Quantum Cryptography (PQC) support in .NET 10, including practical examples and comprehensive documentation.

## Project Contents

### Core Sample
- **Program.cs** - Main demonstration console application
  - ML-KEM (Key Encapsulation Mechanism) example
  - ML-DSA (Digital Signature Algorithm) example
  - Performance benchmarks comparing PQC with traditional RSA/ECDSA
  - Platform support checks
  - Size comparisons

### Documentation Files

#### 1. **README.md** - Quick Start Guide
- Overview of Post-Quantum Cryptography
- Comparison between traditional and PQC algorithms
- Platform support requirements
- Algorithm variants and their uses
- Migration path recommendations
- References and further reading

#### 2. **PQC_API_COMPARISON.md** - API Reference Guide
- Side-by-side comparison of traditional vs PQC APIs
- Detailed API patterns for each operation
- Complete working examples for ML-KEM and ML-DSA
- Performance characteristics
- Security levels explanation
- Migration checklist

#### 3. **PRACTICAL_EXAMPLES.md** - Real-World Scenarios
- Example 1: Secure Message Exchange with ML-KEM
  - Two-party secure communication
  - Combining with AES for full encryption

- Example 2: Document Signing with ML-DSA
  - Creating and verifying digital signatures
  - Tamper detection

- Example 3: Hybrid Cryptography
  - Combining traditional and PQC algorithms
  - Transition strategy

- Example 4: Key Rotation Strategy
  - Planning quantum-safe key lifecycle

- Example 5: Web Service Scenario
  - REST API authentication with PQC
  - Token verification

## Key Learnings

### What's Different About PQC

1. **Generation**: Uses static `GenerateKey()` method instead of constructors
2. **Imports**: Uses static `ImportFromPem()` instead of instance methods
3. **Encapsulation**: ML-KEM provides dedicated `Encapsulate()`/`Decapsulate()`
4. **Simplicity**: No hash algorithm selection for signatures (built-in)
5. **Size**: Larger keys and signatures (1000+ bytes vs 256-512 bytes)

### Security Advantages

- ✓ Resistant to quantum computer attacks
- ✓ NIST standardized (FIPS 203, FIPS 204)
- ✓ No quantum hardware required
- ✓ Comparable performance to traditional algorithms
- ✗ Larger keys/signatures (expected trade-off)
- ✗ Newer standard (less deployment history)

### Recommended Algorithms

| Use Case | Algorithm |
|----------|-----------|
| Key Exchange | ML-KEM-768 (NIST Level 3) |
| Digital Signatures | ML-DSA-65 (NIST Level 3) |
| Government/Military | ML-KEM-1024, ML-DSA-87 (NIST Level 5) |
| High Performance | ML-KEM-512 (NIST Level 1) |

## Running the Sample

### Prerequisites
- .NET 10 SDK
- Windows 11 with latest CNG or Linux with OpenSSL 3.5+

### Build and Run
```bash
cd 2026-Copilot/CryptoPQC/CryptoPQC
dotnet run
```

### Expected Output
```
╔════════════════════════════════════════════════════════════╗
║   Post-Quantum Cryptography (PQC) in .NET 10 Sample       ║
╚════════════════════════════════════════════════════════════╝

▶ PQC Background:
  - Traditional RSA/ECDSA rely on mathematical problems that...
  - NIST standardized ML-KEM (FIPS 203) and ML-DSA (FIPS 204)
  - .NET 10 includes native support...

▶ System Support Check:
  - ML-KEM Support: True
  - ML-DSA Support: True

╔════════════════════════════════════════════════════════════╗
║ 1. ML-KEM (Module-Lattice-Based KEM) - FIPS 203          ║
║    Key Encapsulation Mechanism for secure key agreement  ║
╚════════════════════════════════════════════════════════════╝
...
```

## Project Structure

```
2026-Copilot/CryptoPQC/
├── README.md                          # Main documentation
├── PQC_API_COMPARISON.md              # API reference guide
├── PRACTICAL_EXAMPLES.md              # Real-world code examples
├── PROJECT_OVERVIEW.md                # This file
└── CryptoPQC/
    ├── CryptoPQC.csproj              # .NET 10 console project
    ├── Program.cs                    # Main sample demonstration
    └── bin/obj/                      # Build artifacts
```

## Important Notes

### Experimental APIs
- PQC APIs are marked as experimental in .NET 10
- The sample uses `#pragma warning disable SYSLIB5006` to suppress warnings
- In production code, carefully manage these warnings
- Be prepared for potential API changes before stabilization

### Platform Support
- **Windows**: Requires Windows 11 with latest CNG updates
- **Linux**: Requires OpenSSL 3.5.0 or newer
- **macOS**: Not supported yet

### Migration Strategy

1. **Audit Phase**: Identify all asymmetric cryptography usage
2. **Planning Phase**: Evaluate hybrid approaches
3. **Pilot Phase**: Test on non-critical systems
4. **Rollout Phase**: Gradual deployment with monitoring
5. **Maintenance Phase**: Key rotation and algorithm updates

## Next Steps

1. Review the sample code in `Program.cs`
2. Read `README.md` for overview and benefits
3. Check `PQC_API_COMPARISON.md` for detailed API changes
4. Study examples in `PRACTICAL_EXAMPLES.md` for your use case
5. Run the sample to see PQC in action
6. Implement in your own projects with proper error handling

## Additional Resources

- [NIST Post-Quantum Cryptography](https://csrc.nist.gov/projects/post-quantum-cryptography/)
- [ML-KEM FIPS 203](https://csrc.nist.gov/pubs/fips/203/final)
- [ML-DSA FIPS 204](https://csrc.nist.gov/pubs/fips/204/final)
- [Microsoft .NET Cryptography](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-10/libraries#cryptography)
- [Post-Quantum Cryptography Overview](https://www.cloudflare.com/learning/ssl/post-quantum-cryptography/)

## Troubleshooting

### Build Issues
```bash
# Clean build
dotnet clean
dotnet build

# Check .NET version
dotnet --version  # Should be 10.0.x
```

### Runtime Issues
- **"PQC algorithms not supported on this system"**: Upgrade Windows 11 or OpenSSL to 3.5+
- **SYSLIB5006 warnings**: Expected for experimental APIs; can be suppressed if needed
- **Performance concerns**: Benchmarks are provided in the sample for comparison

## Contributing

This is an educational sample. To improve it:
1. Add more algorithm variants examples
2. Provide cross-platform test results
3. Add hybrid mode examples
4. Include performance profiling data
5. Create unit tests for the operations

---

**Last Updated**: 2024
**Target Framework**: .NET 10
**C# Version**: 14.0
**Status**: Working / Production-Ready for demonstration
