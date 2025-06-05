# Encoder Tool

This repository contains a Python script that obfuscates string and integer literals in C# source files.

## Usage

```bash
./encode_cs.py <input_folder> <output_folder>
```

The script copies the project structure from `input_folder` to `output_folder` and replaces every string literal and integer with a call to helper methods defined in `Decoder.cs`.
After generating the files, it attempts to compile them using the `dotnet` SDK.

## Decoder.cs

`Decoder.cs` provides methods used at runtime to decode the encoded values.
