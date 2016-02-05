Interactive Service HOWTO
===================

Quick usage:

* Add the following values to registry key `HKLM\Software\OpenVPN`:
  * `manual_startup` (string): semicolon separated list of config file names, e.g. `client1.ovpn;client2.ovpn`
  * `user_control` (string): semicolon separated list of config file names, e.g. `client1.ovpn;client2.ovpn`
* Test the program:

  C:\...\openvpnserv2>ServiceControllerExample\bin\Debug\openvpnctl.exe list
  Available configuration files:
  C:\Program Files\OpenVPN\config\client7.ovpn
  Running configurations:

  C:\...\openvpnserv2>tasklist /fi "imagename eq openvpn.exe"
  INFO: No tasks are running which match the specified criteria.
  
  C:\...\openvpnserv2>ServiceControllerExample\bin\Debug\openvpnctl.exe start  "C:\Program Files\OpenVPN\config\client7.ovpn"
  True
  
  C:\...\openvpnserv2>tasklist /fi "imagename eq openvpn.exe"
  
  Image Name                     PID Session Name        Session#    Mem Usage
  ========================= ======== ================ =========== ============
  openvpn.exe                  10968 Services                   0      7,444 K
  
  
  C:\...\openvpnserv2>ServiceControllerExample\bin\Debug\openvpnctl.exe stop  "C:\Program Files\OpenVPN\config\client7.ovpn"
  True
  
  C:\...\openvpnserv2>tasklist /fi "imagename eq openvpn.exe"
  INFO: No tasks are running which match the specified criteria.
  
  C:\...\openvpnserv2>

`openvpnctl`
------------

### `openvpnctl list`
Lists all available configurations

### `openvpnctl start|stop &lt;config file name>
Starts/Stops the VPN specified by this config file. Only config files listed in `openvpnctl list` can be started
(these are config files in the OpenVPN installation directory).
If you are running this from a non-administrator command line, only configs listed under `user_control`
can be started.

