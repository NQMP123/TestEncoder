#!/usr/bin/env python3
import argparse
import base64
import os
import re
import subprocess
from pathlib import Path

STRING_RE = re.compile(r'"(?:\\.|[^"\\])*"')
COMMENT_RE = re.compile(r'//.*?$|/\*.*?\*/', re.DOTALL | re.MULTILINE)
NUMBER_RE = re.compile(r'\b-?\d+\b')

INT_KEY = 0x55AA55AA
SHORT_KEY = 0x55AA
LONG_KEY = 0x0123456789ABCDEF


def encode_string(s: str) -> str:
    return base64.b64encode(s.encode()).decode()


def encode_int(n: int) -> int:
    return n ^ INT_KEY


def encode_number(num_str: str) -> str:
    n = int(num_str)
    enc = encode_int(n)
    return f'Decoder.DecodeInt({enc})'


def process_file(src: Path, dst: Path):
    text = src.read_text(encoding='utf-8')
    comments = {}

    def store_comment(match):
        key = f"__COMMENT_{len(comments)}__"
        comments[key] = match.group(0)
        return key

    text_no_comments = COMMENT_RE.sub(store_comment, text)

    def repl_str(m):
        val = m.group(0)[1:-1]
        enc = encode_string(val)
        return f'Decoder.DecodeString("{enc}")'

    def repl_num(m):
        return encode_number(m.group(0))

    replaced = STRING_RE.sub(repl_str, text_no_comments)
    replaced = NUMBER_RE.sub(repl_num, replaced)

    for k, v in comments.items():
        replaced = replaced.replace(k, v)

    dst.parent.mkdir(parents=True, exist_ok=True)
    dst.write_text(replaced, encoding='utf-8')


def copy_and_encode(src_dir: Path, dst_dir: Path):
    for path in src_dir.rglob('*'):
        rel = path.relative_to(src_dir)
        out = dst_dir / rel
        if path.is_dir():
            out.mkdir(parents=True, exist_ok=True)
        elif path.suffix.lower() == '.cs':
            process_file(path, out)
        else:
            out.write_bytes(path.read_bytes())


def compile_output(dst_dir: Path):
    csproj = dst_dir / 'Encoded.csproj'
    csproj.write_text(
        '<Project Sdk="Microsoft.NET.Sdk">\n'
        '  <PropertyGroup>\n'
        '    <TargetFramework>net8.0</TargetFramework>\n'
        '    <OutputType>Library</OutputType>\n'
        '  </PropertyGroup>\n'
        '  <ItemGroup>\n'
        f'    <Compile Include="{dst_dir}/**/*.cs" />\n'
        '  </ItemGroup>\n'
        '</Project>\n',
        encoding='utf-8'
    )
    try:
        subprocess.run(['dotnet', 'build', str(csproj), '-v', 'minimal'], check=True)
    except subprocess.CalledProcessError:
        pass


def main():
    parser = argparse.ArgumentParser(description='Encode C# constants')
    parser.add_argument('src', type=Path)
    parser.add_argument('dst', type=Path)
    args = parser.parse_args()
    copy_and_encode(args.src, args.dst)
    compile_output(args.dst)


if __name__ == '__main__':
    main()
