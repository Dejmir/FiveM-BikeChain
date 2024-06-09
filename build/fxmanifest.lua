fx_version 'cerulean'
game 'gta5'

client_scripts { "BikeChain.client.net.dll", "BikeChain.client.lua" }
server_script "BikeChain.server.net.dll"

ui_page 'ui/index.html'

files{
	"Newtonsoft.Json.dll",
	"config.json",

	"ui/index.html",
	"ui/style.css",
	"ui/script.js"
}