#!/usr/bin/ruby

require "spaceship"


if ARGV.length != 4 then
  puts "sample: ruby add-device-and-generate-profile.rb TM_徐智利 00008020-001154940A08002E ProfileName.mobileprovision UUID.txt"
    exit 1
end

profilename="TM-V2-Dev-Provisioning-Profile"

profile_name=ARGV[0]
puts "[rb] profile name " + profile_name
profile_udid=ARGV[1]
puts "[rb] profile udid " + profile_udid

profile_filename=ARGV[2]
puts "[rb] profile filename " + profile_filename

profile_fileuuidfilename=ARGV[3]
puts "[rb] profile file uuid filename " + profile_fileuuidfilename


puts "[rb] start login"
Spaceship::Portal.login("tengmu.apple.v2@gmail.com", "Tengmu123456")

puts "[rb] select team " + Spaceship::Portal.select_team

puts "[rb] get certificate"
certificate = Spaceship::Portal.certificate.all.first
puts certificate

puts "[rb] add device "

#Spaceship::Portal.device.create!(udid:"442DA9B414245E18938CEACB0846E1A6150574FD",  name:"KY_薛俊")
#Spaceship::Portal.device.create!(udid:"13A72F9621EDE281325A2736E3F95F64DA1EAE8A",  name:"KY_平平")
#Spaceship::Portal.device.create!(udid:"C4AC32C0AA87FB914572C4C64EC9EC4FCE70FEAB",  name:"KY_王晨阳")
#Spaceship::Portal.device.create!(udid:"00008020-001E10461E78003A"               ,  name:"KY_里豪的pad")
#Spaceship::Portal.device.create!(name:"KY_QA_iPadMini3"     ,udid:"B0DC9F5D4333AC1C7708FB94DCAFAE574E0DFB5A")
#Spaceship::Portal.device.create!(name:"KY_QA_iPhone6"       ,udid:"1ED3BE0249EC776985DDE990102DE0C57018263E")	
#Spaceship::Portal.device.create!(name:"KY_QA_iPhone7"       ,udid:"4c924183f620f66c7b33e517e1c58f663def84ce")	
#Spaceship::Portal.device.create!(name:"KY_QA_iPhoneSE"      ,udid:"53a4e93f5288bd39b32d8fc70b54fa009ecfaf2c")	
#Spaceship::Portal.device.create!(name:"KY_QA_iPhone7Plus"   ,udid:"5d2c728bd6680ab869e5b2d4a2fe1a309270b5ad")	
#Spaceship::Portal.device.create!(name:"KY_QA_iPadMini4"     ,udid:"524973eb0f0c14d46b020fcee70206a7f1a869e1")	
#Spaceship::Portal.device.create!(name:"KY_QA_iPhone6s"      ,udid:"F08B678F4BAAAC28610D26B6454901B40368196B")	
#Spaceship::Portal.device.create!(name:"KY_QA_iPadMini3"     ,udid:"8b81f02ae727de3aafe403d71eeae6c02e14ea16")	
#Spaceship::Portal.device.create!(name:"KY_QA_iPhone6s"      ,udid:"d78f27997b0594e05f319fb1e18144e8a7252525")	
#Spaceship::Portal.device.create!(name:"KY_QA_iPadAir2"      ,udid:"002EA18C005DC674BC385D450702CB208B4C5535")	
#Spaceship::Portal.device.create!(name:"KY_QA_iPadMini4"     ,udid:"39f27a492cf6c1bc53a62528301b269574f6a7e5")	
#Spaceship::Portal.device.create!(name:"KY_QA_iPhoneX"       ,udid:"D7F8FDE8E911BC39EE5CE9D5B79B85D6B4E32B7E")	
#Spaceship::Portal.device.create!(name:"KY_QA_iPhone7"       ,udid:"b5a0b9aea1537bca706df5cbced082d85eaa33a7")	
#Spaceship::Portal.device.create!(name:"KY_QA_iPhone7"       ,udid:"241d1e98ac1d38e4e0beea917ffa1b0eef129236")	
#Spaceship::Portal.device.create!(name:"KY_QA_iPhoneX"       ,udid:"4A0E38A41B847C78E835A6A3387271A9403A3410")	
#Spaceship::Portal.device.create!(name:"KY_QA_iPhone8Plus"   ,udid:"41ac0e7232e32124bfc05d1e6c9003202341c044")	
Spaceship::Portal.device.create!(name: profile_name, udid: profile_udid)


profiles = Spaceship::Portal.provisioning_profile.development.all.find_all do |curprofile|# find_by_name!(name: "测试的一些")
    (curprofile.name == profilename)
end
#puts profile.repair!

profiles.each do |curprofile|
    puts "[rb] delete " + curprofile.name
    curprofile.delete!
end

puts "[rb] create profile"
profile = Spaceship::Portal.provisioning_profile.development.create!(
    bundle_id: "com.*",
    certificate: certificate,
    name: profilename)

# AdHoc Profiles will add all devices by default
#profile = Spaceship::Portal.provisioning_profile.ad_hoc.create!(bundle_id: "com.krausefx.app",
# certificate: cert,
#        name: "Profile Name")
puts "[rb] uuid " + profile.uuid

File.write(profile_fileuuidfilename, profile.uuid)
# Store the new profile on the filesystem
File.write(profile_filename, profile.download)

#all_devices = Spaceship::Portal.device.all
#puts all_devices
