BEGIN {
    count = 0;
    print "kill the autossh process";
}

END {
    print "end the autossh process with count ", count;
}

{
    if ($11 != "grep")
    {
        count++;
        system("kill -9 "$2);
        print "kill pid", $2;
    }
}
