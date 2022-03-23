#!/usr/bin/expect
set timeout 2

set shfile [lindex $argv 0]

spawn sh shfile
expect "*password*"
send "123456\r"
interact
