
using System.Security.Cryptography;
using System.Diagnostics;
using System.Text;

using CryptoPQC;

// Post-Quantum Cryptography (PQC) Demonstration
// This sample shows .NET's built-in PQC support available in .NET 10
// and explains how it differs from and improves upon legacy cryptography

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine(
    """
    ╔════════════════════════════════════════════════════════════╗
    ║   Post-Quantum Cryptography (PQC) in .NET 10 Sample       ║
    ╚════════════════════════════════════════════════════════════╝

    """);

Console.WriteLine(
    """
    ▶ PQC Background:
      - Traditional RSA/ECDSA rely on mathematical problems that
        quantum computers could theoretically solve efficiently.
      - Post-Quantum Cryptography resists attacks from both
        classical and quantum computers.
      - NIST standardized ML-KEM (FIPS 203) and ML-DSA (FIPS 204)
      - .NET 10 includes native support with CNG on Windows and
        OpenSSL 3.5+ support on Linux.

    """);

// Check platform support
Console.WriteLine("▶ System Support Check:");
Console.WriteLine($"  - ML-KEM Support: {MLKem.IsSupported}");
Console.WriteLine($"  - ML-DSA Support: {MLDsa.IsSupported}\n");

if (!MLKem.IsSupported || !MLDsa.IsSupported)
{
    Console.WriteLine(
        """
        ⚠️  PQC algorithms not supported on this system.
            Windows 11 with latest CNG or OpenSSL 3.5+ required.

        """);
    return;
}

// Demonstrate the new PQC algorithms
await PQCUtilities.DemonstratePQCKeyEncapsulation();
await PQCUtilities.DemonstratePQCSignatures();

// Show performance comparison
Console.WriteLine(
    """

    ▶ Performance Comparison (Legacy vs PQC):

    """);
await PQCUtilities.ComparePerformance();

// Summary
Console.WriteLine(
    """

    ╔════════════════════════════════════════════════════════════╗
    ║  Key Advantages of PQC over Traditional Cryptography      ║
    ╚════════════════════════════════════════════════════════════╝
      ✓ Quantum-resistant encryption (ML-KEM - FIPS 203)
      ✓ Quantum-resistant digital signatures (ML-DSA - FIPS 204)
      ✓ Cryptographic strength against future quantum computers
      ✓ No quantum hardware required for implementation
      ✓ Built-in to .NET 10, no external dependencies needed
      ✓ NIST-standardized algorithms
      ✗ Slightly larger key and signature sizes (expected trade-off)

    """);


#pragma warning restore SYSLIB5006
