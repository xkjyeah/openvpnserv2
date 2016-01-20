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

