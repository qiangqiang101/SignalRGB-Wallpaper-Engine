export function Name() { return "Wallpaper"; }
export function Version() { return "1.0.0"; }
export function Type() { return "network"; }
export function Publisher() { return "I'm Not MentaL"; }
export function Size() { return [32, 18]; }
export function DefaultPosition() { return [50, 50]; }
export function DefaultScale() { return 1.0; }
/* global
discovery:readonly
controller:readonly
turnOffOnShutdown:readonly
*/
export function ControllableParameters() {
	return [
		{"property":"shutdownColor", "group":"lighting", "label":"Shutdown Color", "min":"0", "max":"360", "type":"color", "default":"#009bde"},
		{"property":"LightingMode", "group":"lighting", "label":"Lighting Mode", "type":"combobox", "values":["Canvas", "Forced"], "default":"Canvas"},
		{"property":"forcedColor", "group":"lighting", "label":"Forced Color", "min":"0", "max":"360", "type":"color", "default":"#009bde"},
	];
}

const vLedPositions = [
	[0,0],[1,0],[2,0],[3,0],[4,0],[5,0],[6,0],[7,0],[8,0],[9,0],[10,0],[11,0],[12,0],[13,0],[14,0],[15,0],[16,0],[17,0],[18,0],[19,0],[20,0],[21,0],[22,0],[23,0],[24,0],[25,0],[26,0],[27,0],[28,0],[29,0],[30,0],[31,0],
	[0,1],[1,1],[2,1],[3,1],[4,1],[5,1],[6,1],[7,1],[8,1],[9,1],[10,1],[11,1],[12,1],[13,1],[14,1],[15,1],[16,1],[17,1],[18,1],[19,1],[20,1],[21,1],[22,1],[23,1],[24,1],[25,1],[26,1],[27,1],[28,1],[29,1],[30,1],[31,1],
	[0,2],[1,2],[2,2],[3,2],[4,2],[5,2],[6,2],[7,2],[8,2],[9,2],[10,2],[11,2],[12,2],[13,2],[14,2],[15,2],[16,2],[17,2],[18,2],[19,2],[20,2],[21,2],[22,2],[23,2],[24,2],[25,2],[26,2],[27,2],[28,2],[29,2],[30,2],[31,2],
	[0,3],[1,3],[2,3],[3,3],[4,3],[5,3],[6,3],[7,3],[8,3],[9,3],[10,3],[11,3],[12,3],[13,3],[14,3],[15,3],[16,3],[17,3],[18,3],[19,3],[20,3],[21,3],[22,3],[23,3],[24,3],[25,3],[26,3],[27,3],[28,3],[29,3],[30,3],[31,3],
	[0,4],[1,4],[2,4],[3,4],[4,4],[5,4],[6,4],[7,4],[8,4],[9,4],[10,4],[11,4],[12,4],[13,4],[14,4],[15,4],[16,4],[17,4],[18,4],[19,4],[20,4],[21,4],[22,4],[23,4],[24,4],[25,4],[26,4],[27,4],[28,4],[29,4],[30,4],[31,4],
	[0,5],[1,5],[2,5],[3,5],[4,5],[5,5],[6,5],[7,5],[8,5],[9,5],[10,5],[11,5],[12,5],[13,5],[14,5],[15,5],[16,5],[17,5],[18,5],[19,5],[20,5],[21,5],[22,5],[23,5],[24,5],[25,5],[26,5],[27,5],[28,5],[29,5],[30,5],[31,5],
	[0,6],[1,6],[2,6],[3,6],[4,6],[5,6],[6,6],[7,6],[8,6],[9,6],[10,6],[11,6],[12,6],[13,6],[14,6],[15,6],[16,6],[17,6],[18,6],[19,6],[20,6],[21,6],[22,6],[23,6],[24,6],[25,6],[26,6],[27,6],[28,6],[29,6],[30,6],[31,6],
	[0,7],[1,7],[2,7],[3,7],[4,7],[5,7],[6,7],[7,7],[8,7],[9,7],[10,7],[11,7],[12,7],[13,7],[14,7],[15,7],[16,7],[17,7],[18,7],[19,7],[20,7],[21,7],[22,7],[23,7],[24,7],[25,7],[26,7],[27,7],[28,7],[29,7],[30,7],[31,7],
	[0,8],[1,8],[2,8],[3,8],[4,8],[5,8],[6,8],[7,8],[8,8],[9,8],[10,8],[11,8],[12,8],[13,8],[14,8],[15,8],[16,8],[17,8],[18,8],[19,8],[20,8],[21,8],[22,8],[23,8],[24,8],[25,8],[26,8],[27,8],[28,8],[29,8],[30,8],[31,8],
	[0,9],[1,9],[2,9],[3,9],[4,9],[5,9],[6,9],[7,9],[8,9],[9,9],[10,9],[11,9],[12,9],[13,9],[14,9],[15,9],[16,9],[17,9],[18,9],[19,9],[20,9],[21,9],[22,9],[23,9],[24,9],[25,9],[26,9],[27,9],[28,9],[29,9],[30,9],[31,9],
	[0,10],[1,10],[2,10],[3,10],[4,10],[5,10],[6,10],[7,10],[8,10],[9,10],[10,10],[11,10],[12,10],[13,10],[14,10],[15,10],[16,10],[17,10],[18,10],[19,10],[20,10],[21,10],[22,10],[23,10],[24,10],[25,10],[26,10],[27,10],[28,10],[29,10],[30,10],[31,10],
	[0,11],[1,11],[2,11],[3,11],[4,11],[5,11],[6,11],[7,11],[8,11],[9,11],[10,11],[11,11],[12,11],[13,11],[14,11],[15,11],[16,11],[17,11],[18,11],[19,11],[20,11],[21,11],[22,11],[23,11],[24,11],[25,11],[26,11],[27,11],[28,11],[29,11],[30,11],[31,11],
	[0,12],[1,12],[2,12],[3,12],[4,12],[5,12],[6,12],[7,12],[8,12],[9,12],[10,12],[11,12],[12,12],[13,12],[14,12],[15,12],[16,12],[17,12],[18,12],[19,12],[20,12],[21,12],[22,12],[23,12],[24,12],[25,12],[26,12],[27,12],[28,12],[29,12],[30,12],[31,12],
	[0,13],[1,13],[2,13],[3,13],[4,13],[5,13],[6,13],[7,13],[8,13],[9,13],[10,13],[11,13],[12,13],[13,13],[14,13],[15,13],[16,13],[17,13],[18,13],[19,13],[20,13],[21,13],[22,13],[23,13],[24,13],[25,13],[26,13],[27,13],[28,13],[29,13],[30,13],[31,13],
	[0,14],[1,14],[2,14],[3,14],[4,14],[5,14],[6,14],[7,14],[8,14],[9,14],[10,14],[11,14],[12,14],[13,14],[14,14],[15,14],[16,14],[17,14],[18,14],[19,14],[20,14],[21,14],[22,14],[23,14],[24,14],[25,14],[26,14],[27,14],[28,14],[29,14],[30,14],[31,14],
	[0,15],[1,15],[2,15],[3,15],[4,15],[5,15],[6,15],[7,15],[8,15],[9,15],[10,15],[11,15],[12,15],[13,15],[14,15],[15,15],[16,15],[17,15],[18,15],[19,15],[20,15],[21,15],[22,15],[23,15],[24,15],[25,15],[26,15],[27,15],[28,15],[29,15],[30,15],[31,15],
	[0,16],[1,16],[2,16],[3,16],[4,16],[5,16],[6,16],[7,16],[8,16],[9,16],[10,16],[11,16],[12,16],[13,16],[14,16],[15,16],[16,16],[17,16],[18,16],[19,16],[20,16],[21,16],[22,16],[23,16],[24,16],[25,16],[26,16],[27,16],[28,16],[29,16],[30,16],[31,16],
	[0,17],[1,17],[2,17],[3,17],[4,17],[5,17],[6,17],[7,17],[8,17],[9,17],[10,17],[11,17],[12,17],[13,17],[14,17],[15,17],[16,17],[17,17],[18,17],[19,17],[20,17],[21,17],[22,17],[23,17],[24,17],[25,17],[26,17],[27,17],[28,17],[29,17],[30,17],[31,17]
];
const vLedNames = [
	"Led1","Led2","Led3","Led4","Led5","Led6","Led7","Led8","Led9","Led10","Led11","Led12","Led13","Led14","Led15","Led16","Led17","Led18","Led19","Led20","Led21","Led22","Led23","Led24","Led25","Led26","Led27",
	"Led28","Led29","Led30","Led31","Led32","Led33","Led34","Led35","Led36","Led37","Led38","Led39","Led40","Led41","Led42","Led43","Led44","Led45","Led46","Led47","Led48","Led49","Led50","Led51","Led52","Led53",
	"Led54","Led55","Led56","Led57","Led58","Led59","Led60","Led61","Led62","Led63","Led64","Led65","Led66","Led67","Led68","Led69","Led70","Led71","Led72","Led73","Led74","Led75","Led76","Led77","Led78","Led79",
	"Led80","Led81","Led82","Led83","Led84","Led85","Led86","Led87","Led88","Led89","Led90","Led91","Led92","Led93","Led94","Led95","Led96","Led97","Led98","Led99","Led100","Led101","Led102","Led103","Led104",
	"Led105","Led106","Led107","Led108","Led109","Led110","Led111","Led112","Led113","Led114","Led115","Led116","Led117","Led118","Led119","Led120","Led121","Led122","Led123","Led124","Led125","Led126","Led127",
	"Led128","Led129","Led130","Led131","Led132","Led133","Led134","Led135","Led136","Led137","Led138","Led139","Led140","Led141","Led142","Led143","Led144","Led145","Led146","Led147","Led148","Led149","Led150",
	"Led151","Led152","Led153","Led154","Led155","Led156","Led157","Led158","Led159","Led160","Led161","Led162","Led163","Led164","Led165","Led166","Led167","Led168","Led169","Led170","Led171","Led172","Led173",
	"Led174","Led175","Led176","Led177","Led178","Led179","Led180","Led181","Led182","Led183","Led184","Led185","Led186","Led187","Led188","Led189","Led190","Led191","Led192","Led193","Led194","Led195","Led196",
	"Led197","Led198","Led199","Led200","Led201","Led202","Led203","Led204","Led205","Led206","Led207","Led208","Led209","Led210","Led211","Led212","Led213","Led214","Led215","Led216","Led217","Led218","Led219",
	"Led220","Led221","Led222","Led223","Led224","Led225","Led226","Led227","Led228","Led229","Led230","Led231","Led232","Led233","Led234","Led235","Led236","Led237","Led238","Led239","Led240","Led241","Led242",
	"Led243","Led244","Led245","Led246","Led247","Led248","Led249","Led250","Led251","Led252","Led253","Led254","Led255","Led256","Led257","Led258","Led259","Led260","Led261","Led262","Led263","Led264","Led265",
	"Led266","Led267","Led268","Led269","Led270","Led271","Led272","Led273","Led274","Led275","Led276","Led277","Led278","Led279","Led280","Led281","Led282","Led283","Led284","Led285","Led286","Led287","Led288",
	"Led289","Led290","Led291","Led292","Led293","Led294","Led295","Led296","Led297","Led298","Led299","Led300","Led301","Led302","Led303","Led304","Led305","Led306","Led307","Led308","Led309","Led310","Led311",
	"Led312","Led313","Led314","Led315","Led316","Led317","Led318","Led319","Led320","Led321","Led322","Led323","Led324","Led325","Led326","Led327","Led328","Led329","Led330","Led331","Led332","Led333","Led334",
	"Led335","Led336","Led337","Led338","Led339","Led340","Led341","Led342","Led343","Led344","Led345","Led346","Led347","Led348","Led349","Led350","Led351","Led352","Led353","Led354","Led355","Led356","Led357",
	"Led358","Led359","Led360","Led361","Led362","Led363","Led364","Led365","Led366","Led367","Led368","Led369","Led370","Led371","Led372","Led373","Led374","Led375","Led376","Led377","Led378","Led379","Led380",
	"Led381","Led382","Led383","Led384","Led385","Led386","Led387","Led388","Led389","Led390","Led391","Led392","Led393","Led394","Led395","Led396","Led397","Led398","Led399","Led400","Led401","Led402","Led403",
	"Led404","Led405","Led406","Led407","Led408","Led409","Led410","Led411","Led412","Led413","Led414","Led415","Led416","Led417","Led418","Led419","Led420","Led421","Led422","Led423","Led424","Led425","Led426",
	"Led427","Led428","Led429","Led430","Led431","Led432","Led433","Led434","Led435","Led436","Led437","Led438","Led439","Led440","Led441","Led442","Led443","Led444","Led445","Led446","Led447","Led448","Led449",
	"Led450","Led451","Led452","Led453","Led454","Led455","Led456","Led457","Led458","Led459","Led460","Led461","Led462","Led463","Led464","Led465","Led466","Led467","Led468","Led469","Led470","Led471","Led472",
	"Led473","Led474","Led475","Led476","Led477","Led478","Led479","Led480","Led481","Led482","Led483","Led484","Led485","Led486","Led487","Led488","Led489","Led490","Led491","Led492","Led493","Led494","Led495",
	"Led496","Led497","Led498","Led499","Led500","Led501","Led502","Led503","Led504","Led505","Led506","Led507","Led508","Led509","Led510","Led511","Led512","Led513","Led514","Led515","Led516","Led517","Led518",
	"Led519","Led520","Led521","Led522","Led523","Led524","Led525","Led526","Led527","Led528","Led529","Led530","Led531","Led532","Led533","Led534","Led535","Led536","Led537","Led538","Led539","Led540","Led541",
	"Led542","Led543","Led544","Led545","Led546","Led547","Led548","Led549","Led550","Led551","Led552","Led553","Led554","Led555","Led556","Led557","Led558","Led559","Led560","Led561","Led562","Led563","Led564",
	"Led565","Led566","Led567","Led568","Led569","Led570","Led571","Led572","Led573","Led574","Led575","Led576"
];

export function LedNames() {
	return vLedNames;
}

export function LedPositions() {
	return vLedPositions;
}

export function Initialize() {
	device.setName(controller.name);
	device.addFeature("udp");
}

export function Render() {
	grabColors();
}

export function Shutdown(suspend) {
	grabColors(true);
}

function grabColors(shutdown = false) {
	const RGBData = [];

	for(let iIdx = 0; iIdx < vLedPositions.length; iIdx++) {
		const iPxX = vLedPositions[iIdx][0];
		const iPxY = vLedPositions[iIdx][1];
		let color;

		if(shutdown) {
			color = hexToRgb(shutdownColor);
		} else if (LightingMode === "Forced") {
			color = hexToRgb(forcedColor);
		} else {
			color = device.color(iPxX, iPxY);
		}

		const iLedIdx = iIdx * 3;
		RGBData[iLedIdx] = color[0];
		RGBData[iLedIdx+1] = color[1];
		RGBData[iLedIdx+2] = color[2];
	}

	udp.send(controller.ip, controller.port, RGBData);
}

function hexToRgb(hex) {
	const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
	const colors = [];
	colors[0] = parseInt(result[1], 16);
	colors[1] = parseInt(result[2], 16);
	colors[2] = parseInt(result[3], 16);

	return colors;
}

export function DiscoveryService() {

	this.Initialize = () => {
		service.addController(new Wallpaper({
			id: "Wallpaper",
			port: 8123,
			ip: "127.0.0.1",
			name: "Wallpaper",
		}));

		const controller = service.getController("Wallpaper");

		service.updateController(controller);
		service.announceController(controller);
	};

	this.Update = () => {

		for(const cont of service.controllers) {
			cont.obj.update();
		}
	};
}



class Wallpaper {
	constructor(value) {
		this.id = value.id;
		this.port = value.port;
		this.ip = value.ip;
		this.name = value.name;

		this.initialized = false;
	}

	update(){
		if(!this.initialized){
			this.initialized = true;

			service.updateController(this);
			service.announceController(this);
		}
	}
}

export function ImageUrl() {
	return "https://i.imgur.com/yknEGHA.png";
}