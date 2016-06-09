OpenVPN Service
==============

Same concept as https://openvpnwinsvc.codeplex.com/ but more similar to the old
`openvpnserv.exe`. Intended as a drop-in replacement for the latter with
suspend/resume support and auto-restart support. Parsing of the configuration
file is still left to `openvpn.exe`.

Aim of this package is to fix:
- [#595](https://community.openvpn.net/openvpn/ticket/595)
- [#71](https://community.openvpn.net/openvpn/ticket/71)

Pre-built binaries are included in `bin/`

Building on Linux
-----------------

This package can be built under Linux.

On Ubuntu 14.04, you need the following packages and their dependencies:

- `mono-devel`
- `mono-xbuild`
- `libmono-microsoft-build-tasks-v4.0-4.0-cil`
- `libmono-system-serviceprocess4.0-cil`
- `libmono-system-management4.0-cil`

A simple `./build.sh` should be sufficient to generate the binaries.

