Tech - VS MVC Core (8.0)

AES Encryption -
AES (Advanced Encryption Standard) is a symmetric encryption algorithm widely used for securing data. It uses the same key for both encryption and decryption, making it efficient for handling large amounts of data.

Key Points:
Key Sizes: 128, 192, or 256 bits.
Block Size: 128 bits.
Encryption: Encrypts data in 128-bit blocks.
Decryption: Uses the same key to decrypt data.
Key Operations:
Generate Key and IV: Create a key and initialization vector (IV).
Encrypt: Use the key and IV to encrypt data.
Decrypt: Use the same key and IV to decrypt data.

Triple Des - 
Key Generation: Generate three unique 56-bit keys. Combine them to form a 168-bit key for Triple DES encryption.
Encryption:
Encrypt the plaintext with the first key.
Decrypt the result with the second key.
Encrypt the result again with the third key.
Decryption follows the reverse process:
Decrypt the ciphertext with the third key.
Encrypt the result with the second key.
Decrypt the result again with the first key

To run the project update-database