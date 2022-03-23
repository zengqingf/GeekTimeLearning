//
//  GCloudVoiceVersion.h
//  gcloud_voice
//
//  Created by cz on 15/10/19.
//  Copyright 2015 gcloud. All rights reserved.
//

#ifndef gcloud_voice_GCloudVoiceVersion_h_
#define gcloud_voice_GCloudVoiceVersion_h_

#define GCLOUD_VOICE_VERSION "3.1.0.149472631"
#define GCLOUD_VOICE_JAVA_VERSION "3.1.0.149472631"

namespace gcloud_voice
{
	static const int VER_MAJOR = 3;
	static const int VER_MINOR = 1;
	static const int VER_FIX   = 0;
	static const int VER_GIT   = 149472631;

	static const char *ID_GIT = "8e8c577";

	static const int VER_BUF_LEN = 1024;

	const char * gvoice_get_version();

	const char * gvoice_get_full_version();
}
#endif /* gcloud_voice_GCloudVoiceVersion_h_ */
