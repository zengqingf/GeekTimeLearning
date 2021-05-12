using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileIO_1
{
    class Program
    {
        static void Main(string[] args)
        {
            //e.g. F:\_Dev\projects\Tenmove_Project_A8_trunk\Client\NextGenGame\Saved\StagedBuilds\IOS\cookeddata\nextgenactiongame\content\paks\nextgenactiongame-ios.pak
            string absoluteFilename = @"F:\_Dev\projects\Tenmove_Project_A8_trunk\Client\NextGenGame\Saved\StagedBuilds\IOS\cookeddata\nextgenactiongame\content\paks\nextgenactiongame-ios.pak";
            byte[] fileContents = FileIO.ReadLargeFile(absoluteFilename);
            if (fileContents.Length != 0)
            {
                FileIO.WriteFile(absoluteFilename + ".temp", fileContents);
            }
        }
    }
}
