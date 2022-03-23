#pragma once

#include "uwa.h"
#include<fstream>

// a simple class to help save data. can use fwrite to replace
class DLL_API FileWriter
{
public:
	FileWriter();
	~FileWriter();
	FileWriter(const char* filename );
	FileWriter(const char* filename, int mode);
	FileWriter(const FileWriter&) = delete;
	FileWriter(FileWriter&&) = default;
	FileWriter& operator= (const FileWriter&) = delete;
	FileWriter& operator= (FileWriter&&) = default;
	explicit operator bool() const;

	FileWriter& operator<<(const char * rhs);
	FileWriter& operator<<(const uint8& rhs);

	void Println(const char * str);

	bool Open(const char* filename, int mode = 2);
	void Flush();
private:
	struct FileWriterImpl;
	FileWriterImpl* writer;
};