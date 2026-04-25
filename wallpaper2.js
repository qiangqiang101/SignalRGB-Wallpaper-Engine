export function Name() { return "Wallpaper Engine 2"; }
export function Version() { return "2.0.0"; }
export function Type() { return "network"; }
export function Publisher() { return "I'm Not MentaL"; }
export function Size() { return [32, 18]; }
export function DefaultPosition() { return [50, 50]; }
export function DefaultScale() { return 0.2; }
export function DeviceType() { return "wifi"; }
/* global
discovery:readonly
controller:readonly
ShutdownEffect:readonly
shutdownColor:readonly
LightingMode:readonly
forcedColor:readonly
MatrixSize:readonly
MatrixTier:readonly
BlurIntensity:readonly
LedShape:readonly
RoundedRectangleCornerRadius:readonly
LedPadding:readonly
FPS:readonly
ShowFps:readonly
BackgroundColor:readonly
CpuUsagePauseValue:readonly
CoverImage:readonly
CoverImageStretch:readonly
*/
export function ControllableParameters() {
	return [
		{ "property": "ShutdownEffect", "group": "lighting", "label": "Shutdown Effect", description: "This mode is applied to the device when the System, or SignalRGB is shutting down.", "type": "combobox", "values": ["Solid Color", "Aurora", "Breathing", "Rainbow Wave (Left)", "Rainbow Wave (Right)", "Neon Wave (Left)", "Neon Wave (Right)", "Sunset Wave (Left)", "Sunset Wave (Right)", "Audio Party", "Rainbow Cycle", "Rainbow Pinwheel", "Fire"], "default": "Solid Color" },
		{ "property": "shutdownColor", "group": "lighting", "label": "Shutdown Color", description: "This color is applied to the device when the System, or SignalRGB is shutting down.", "min": "0", "max": "360", "type": "color", "default": "#009bde" },
		{ "property": "LightingMode", "group": "lighting", "label": "Lighting Mode", description: "Determines where the device's RGB comes from. Canvas will pull from the active Effect, while Forced will override it to a specific color", "type": "combobox", "values": ["Canvas", "Forced"], "default": "Canvas" },
		{ "property": "forcedColor", "group": "lighting", "label": "Forced Color", description: "The color used when 'Forced' Lighting Mode is enabled", "min": "0", "max": "360", "type": "color", "default": "#009bde" },
		{ "property": "MatrixSize", "group": "settings", "label": "Aspect Ratio", description: "Choose your screen's aspect ratio.", "type": "combobox", "values": ["4:1 Landscape", "4:1 Portrait", "4:3 Landscape", "4:3 Portrait", "5:4 Landscape", "5:4 Portrait", "16:9 Landscape", "16:9 Portrait", "16:10 Landscape", "16:10 Portrait", "21:9 Landscape", "21:9 Portrait", "32:9 Landscape", "32:9 Portrait"], "default": "16:9 Landscape" },
		{ "property": "MatrixTier", "group": "settings", "label": "Display Size", description: "Choose the display size tier. Standard provides a balanced grid for typical setups, while Large increases the grid for more detail and a bigger LED canvas.", "type": "combobox", "values": ["Small", "Normal", "Large", "X Large"], "default": "Normal" },
		{ "property": "BlurIntensity", "group": "lighting", "label": "Blur Intensity", description: "Blur Intensity controls how soft or diffused the LEDs look. Lower values keep them sharp and defined, while higher values make them glow with a frosted, hazy effect.", "step": "1", "type": "number", "min": "0", "max": "100", "default": "20" },
		{ "property": "LedShape", "group": "lighting", "label": "LED Shape", description: "Choose your preferred LED shape.", "type": "combobox", "values": ["Rectangle", "Rounded Rectangle", "Sphere"], "default": "Rectangle" },
		{ "property": "RoundedRectangleCornerRadius", "group": "lighting", description: "Specifies how round you want your LED's corner. (Only works when LED Shape is Rounded Rectangle)", "label": "Rounded Rectangle Corner Radius", "step": "1", "type": "number", "min": "0", "max": "20", "default": "2" },
		{ "property": "LedPadding", "label": "LED Padding", "group": "lighting", description: "Specifies how much spacing between LEDs.", "step": "1", "type": "number", "min": "0", "max": "250", "default": "0" },
		{ "property": "FPS", "label": "Target FPS", description: "Number of rendered frames per second.", "step": "1", "type": "number", "min": "1", "max": "240", "default": "60" },
		{ "property": "ShowFps", "group": "settings", "label": "Show FPS", description: "Displays the current frames per second (FPS) of the wallpaper animation, useful for nerds.", "type": "boolean", "default": "false" },
		{ "property": "BackgroundColor", "group": "settings", "label": "Background Color", description: "This color is applied to the background.", "min": "0", "max": "360", "type": "color", "default": "#000000" },
		// { "property": "CpuUsagePauseValue", "label": "CPU Usage Pause Value", description: "Temporary Pause Wallpaper effects to reduce CPU usage for potato PC", "step": "1", "type": "number", "min": "50", "max": "100", "default": "60" },
		{ "property": "CoverImage", "label": "Cover Image", "type": "textfield", description: "A diffuser image to cover the virtual LEDs, supports local and web files. (Example: 'C:\\Users\\BARRY\\Pictures\\razer2.png' or 'https://github.com/qiangqiang101/OpenRGB-Wallpaper/raw/master/Wallpaper-Wallpaper/razer5.png')", "default": "https://github.com/qiangqiang101/OpenRGB-Wallpaper/raw/master/Wallpaper-Wallpaper/razer5.png?raw=true" },
		{ "property": "CoverImageStretch", "group": "settings", "label": "Cover Image Stretch", description: "Specifies how an image is positioned.", "type": "combobox", "values": ["None", "Fill", "Uniform", "Uniform to Fill"], "default": "Uniform" },
	];
}

//Fixed settings
const MaxLedsInPacket = 480; //483;
const ColorPacket = 0x00;
const SettingPacket = 0x01;

//User settings
const vMatrixSize = { "4:3 Landscape": 0, "4:3 Portrait": 1, "5:4 Landscape": 2, "5:4 Portrait": 3, "16:9 Landscape": 4, "16:9 Portrait": 5, "16:10 Landscape": 6, "16:10 Portrait": 7, "21:9 Landscape": 8, "21:9 Portrait": 9, "32:9 Landscape": 10, "32:9 Portrait": 11, "4:1 Landscape": 12, "4:1 Portrait": 13 };
const vMatrixTier = { "Small": 0, "Normal": 1, "Large": 2, "X Large": 3 };
const vShutdownEffect = { 
	"Solid Color": 0, "Aurora": 1, "Breathing": 2, "Rainbow Wave (Left)": 3, "Rainbow Wave (Right)": 4, "Neon Wave (Left)": 5, "Neon Wave (Right)": 6, 
	"Sunset Wave (Left)": 7, "Sunset Wave (Right)": 8, "Audio Party": 9, "Rainbow Cycle": 10, "Rainbow Pinwheel": 11, "Fire": 12
};
const vLedShape = { "Rectangle": 0, "Rounded Rectangle": 1, "Sphere": 2 };
const vCoverImageStretch = { "None": 0, "Fill": 1, "Uniform": 2, "Uniform to Fill": 3 };
const vLedPositions = {
	"4:1 Landscape": { "Small": generateLedPositions(8, 2), "Normal": generateLedPositions(16, 4), "Large": generateLedPositions(32, 8), "X Large": generateLedPositions(64, 16) },
	"4:1 Portrait": { "Small": generateLedPositions(2, 8), "Normal": generateLedPositions(4, 16), "Large": generateLedPositions(8, 32), "X Large": generateLedPositions(16, 64) },
	"4:3 Landscape": { "Small": generateLedPositions(8, 6), "Normal": generateLedPositions(16, 12), "Large": generateLedPositions(32, 24), "X Large": generateLedPositions(64, 48) },
	"4:3 Portrait": { "Small": generateLedPositions(6, 8), "Normal": generateLedPositions(12, 16), "Large": generateLedPositions(24, 32), "X Large": generateLedPositions(48, 64) },
	"5:4 Landscape": { "Small": generateLedPositions(10, 8), "Normal": generateLedPositions(20, 16), "Large": generateLedPositions(40, 32), "X Large": generateLedPositions(80, 64) },
	"5:4 Portrait": { "Small": generateLedPositions(8, 10), "Normal": generateLedPositions(16, 20), "Large": generateLedPositions(32, 40), "X Large": generateLedPositions(64, 80) },
	"16:9 Landscape": { "Small": generateLedPositions(32, 18), "Normal": generateLedPositions(48, 27), "Large": generateLedPositions(64, 36), "X Large": generateLedPositions(128, 72) },
	"16:9 Portrait": { "Small": generateLedPositions(18, 32), "Normal": generateLedPositions(27, 48), "Large": generateLedPositions(36, 64), "X Large": generateLedPositions(72, 128) },
	"16:10 Landscape": { "Small": generateLedPositions(32, 20), "Normal": generateLedPositions(48, 30), "Large": generateLedPositions(64, 40), "X Large": generateLedPositions(128, 80) },
	"16:10 Portrait": { "Small": generateLedPositions(29, 32), "Normal": generateLedPositions(30, 48), "Large": generateLedPositions(40, 64), "X Large": generateLedPositions(80, 128) },
	"21:9 Landscape": { "Small": generateLedPositions(42, 18), "Normal": generateLedPositions(63, 27), "Large": generateLedPositions(84, 36), "X Large": generateLedPositions(168, 72) },
	"21:9 Portrait": { "Small": generateLedPositions(18, 42), "Normal": generateLedPositions(27, 63), "Large": generateLedPositions(36, 84), "X Large": generateLedPositions(72, 168) },
	"32:9 Landscape": { "Small": generateLedPositions(64, 18), "Normal": generateLedPositions(96, 27), "Large": generateLedPositions(128, 36), "X Large": generateLedPositions(256, 72) },
	"32:9 Portrait": { "Small": generateLedPositions(18, 64), "Normal": generateLedPositions(27, 96), "Large": generateLedPositions(36, 128), "X Large": generateLedPositions(72, 256) }
};
const vLedSizes = {
	"4:1 Landscape": { "Small": [8, 2], "Normal": [16, 4], "Large": [32, 8], "X Large": [64, 16] },
	"4:1 Portrait": { "Small": [2, 8], "Normal": [4, 16], "Large": [8, 32], "X Large": [16, 64] },
	"4:3 Landscape": { "Small": [8, 6], "Normal": [16, 12], "Large": [32, 24], "X Large": [64, 48] },
	"4:3 Portrait": { "Small": [6, 8], "Normal": [12, 16], "Large": [24, 32], "X Large": [48, 64] },
	"5:4 Landscape": { "Small": [10, 8], "Normal": [20, 16], "Large": [40, 32], "X Large": [80, 64] },
	"5:4 Portrait": { "Small": [8, 10], "Normal": [16, 20], "Large": [32, 40], "X Large": [64, 80] },
	"16:9 Landscape": { "Small": [32, 18], "Normal": [48, 27], "Large": [64, 36], "X Large": [128, 72] },
	"16:9 Portrait": { "Small": [18, 32], "Normal": [27, 48], "Large": [36, 64], "X Large": [72, 128] },
	"16:10 Landscape": { "Small": [32, 20], "Normal": [48, 30], "Large": [64, 40], "X Large": [128, 80] },
	"16:10 Portrait": { "Small": [20, 32], "Normal": [30, 48], "Large": [40, 64], "X Large": [80, 128] },
	"21:9 Landscape": { "Small": [42, 18], "Normal": [63, 27], "Large": [84, 36], "X Large": [168, 72] },
	"21:9 Portrait": { "Small": [18, 42], "Normal": [27, 63], "Large": [36, 84], "X Large": [72, 168] },
	"32:9 Landscape": { "Small": [64, 18], "Normal": [96, 27], "Large": [128, 36], "X Large": [256, 72] },
	"32:9 Portrait": { "Small": [18, 64], "Normal": [27, 96], "Large": [36, 128], "X Large": [72, 256] }
}
const _vLedNames = {
	"4:1": { "Small": generateLedNames(16), "Normal": generateLedNames(64), "Large": generateLedNames(256), "X Large": generateLedNames(1024) },
	"4:3": { "Small": generateLedNames(48), "Normal": generateLedNames(192), "Large": generateLedNames(768), "X Large": generateLedNames(3072) },
	"5:4": { "Small": generateLedNames(80), "Normal": generateLedNames(320), "Large": generateLedNames(1280), "X Large": generateLedNames(5120) },
	"16:9": { "Small": generateLedNames(576), "Normal": generateLedNames(1296), "Large": generateLedNames(2304), "X Large": generateLedNames(9216) },
	"16:10": { "Small": generateLedNames(640), "Normal": generateLedNames(1440), "Large": generateLedNames(2560), "X Large": generateLedNames(10240) },
	"21:9": { "Small": generateLedNames(756), "Normal": generateLedNames(1701), "Large": generateLedNames(3024), "X Large": generateLedNames(12096) },
	"32:9": { "Small": generateLedNames(1152), "Normal": generateLedNames(2592), "Large": generateLedNames(4608), "X Large": generateLedNames(18432) },
};
const vLedNames = {
	"4:1 Landscape": _vLedNames["4:1"],
	"4:1 Portrait": _vLedNames["4:1"],
	"4:3 Landscape": _vLedNames["4:3"],
	"4:3 Portrait": _vLedNames["4:3"],
	"5:4 Landscape": _vLedNames["5:4"],
	"5:4 Portrait": _vLedNames["5:4"],
	"16:9 Landscape": _vLedNames["16:9"],
	"16:9 Portrait": _vLedNames["16:9"],
	"16:10 Landscape": _vLedNames["16:10"],
	"16:10 Portrait": _vLedNames["16:10"],
	"21:9 Landscape": _vLedNames["21:9"],
	"21:9 Portrait": _vLedNames["21:9"],
	"32:9 Landscape": _vLedNames["32:9"],
	"32:9 Portrait": _vLedNames["32:9"],
};

export function onMatrixSizeChanged() {
	device.setSize(vLedSizes[MatrixSize][MatrixTier]);
	device.setControllableLeds(vLedNames[MatrixSize][MatrixTier], vLedPositions[MatrixSize][MatrixTier]);
	updateSettings();
}

export function onMatrixTierChanged() {
	device.setSize(vLedSizes[MatrixSize][MatrixTier]);
	device.setControllableLeds(vLedNames[MatrixSize][MatrixTier], vLedPositions[MatrixSize][MatrixTier]);
	updateSettings();
}

export function onShutdownEffectChanged() {
	updateSettings();
}

export function onShowFpsChanged() {
	updateSettings();
}

export function onBlurIntensityChanged() {
	updateSettings();
}

export function onLedShapeChanged() {
	updateSettings();
}

export function onRoundedRectangleCornerRadiusChanged() {
	updateSettings();
}

export function onLedPaddingChanged() {
	updateSettings();
}

export function onFPSChanged() {
	updateSettings();
}

export function onCoverImageStretchChanged() {
	updateSettings();
}

export function onCoverImageChanged() {
	updateSettings();
}

export function onBackgroundColorChanged() {
	updateSettings();
}

export function LedNames() {
	return vLedNames[MatrixSize][MatrixTier];
}

export function LedPositions() {
	return vLedPositions[MatrixSize][MatrixTier];
}

export function Initialize() {
	device.setName(controller.name);
	device.addFeature("udp");
	device.setSize(vLedSizes[MatrixSize][MatrixTier]);
	device.setFrameRateTarget(60);
}

export function Render() {
	grabColors();
}

export function Shutdown(suspend) {
	grabColors(true);
}

function updateSettings() {
	let bgcolor = hexToRgb(BackgroundColor);
	let sdcolor = hexToRgb(shutdownColor);
	let packet = [SettingPacket, vMatrixSize[MatrixSize], vMatrixTier[MatrixTier], vShutdownEffect[ShutdownEffect], ShowFps, BlurIntensity,
		vLedShape[LedShape], RoundedRectangleCornerRadius, LedPadding, FPS, vCoverImageStretch[CoverImageStretch], 0, // CpuUsagePauseValue
		bgcolor[0], bgcolor[1], bgcolor[2], sdcolor[0], sdcolor[1], sdcolor[2]]; //0x01 = Send Settings

	// Convert string to bytes and add to packet
	const coverImageBytes = stringToBytes(CoverImage);
	packet.push(coverImageBytes.length); // Length prefix
	packet.push(...coverImageBytes);

	udp.send(controller.ip, controller.port, packet);
}

function grabColors(shutdown = false) {
	const RGBData = [];
	const LedCount = (vLedSizes[MatrixSize][MatrixTier][0] * vLedSizes[MatrixSize][MatrixTier][1]); //1296
	const NumPackets = Math.ceil(LedCount / MaxLedsInPacket); //2

	for (let iIdx = 0; iIdx < vLedPositions[MatrixSize][MatrixTier].length; iIdx++) {
		const iPxX = vLedPositions[MatrixSize][MatrixTier][iIdx][0];
		const iPxY = vLedPositions[MatrixSize][MatrixTier][iIdx][1];
		let color;

		if (shutdown) {
			color = hexToRgb(shutdownColor);
		} else if (LightingMode === "Forced") {
			color = hexToRgb(forcedColor);
		} else {
			color = device.color(iPxX, iPxY);
		}

		const iLedIdx = (iIdx) * 3;
		RGBData[iLedIdx] = color[0];
		RGBData[iLedIdx + 1] = color[1];
		RGBData[iLedIdx + 2] = color[2];
	}

	for (let currPacket = 0; currPacket < NumPackets; currPacket++) {
		const startIdx = currPacket * MaxLedsInPacket;
		let packet = [ColorPacket, currPacket, NumPackets];
		packet = packet.concat(RGBData.splice(0, MaxLedsInPacket * 3));
		udp.send(controller.ip, controller.port, packet);
	}
}

function stringToBytes(str) {
	const bytes = [];
	for (let i = 0; i < str.length; i++) {
		const code = str.charCodeAt(i);
		if (code < 0x80) {
			bytes.push(code);
		} else if (code < 0x800) {
			bytes.push(0xc0 | (code >> 6));
			bytes.push(0x80 | (code & 0x3f));
		} else if (code < 0xd800 || code >= 0xe000) {
			bytes.push(0xe0 | (code >> 12));
			bytes.push(0x80 | ((code >> 6) & 0x3f));
			bytes.push(0x80 | (code & 0x3f));
		}
	}
	return bytes;
}

function hexToRgb(hex) {
	const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
	const colors = [];
	colors[0] = parseInt(result[1], 16);
	colors[1] = parseInt(result[2], 16);
	colors[2] = parseInt(result[3], 16);

	return colors;
}

function generateLedPositions(width, height) {
	const positions = [];

	for (let y = 0; y < height; y++) {
		for (let x = 0; x < width; x++) {
			positions.push([x, y]);
		}
	}

	return positions;
};

function generateLedNames(count) {
	const names = [];

	for (let i = 1; i <= count; i++) {
		names.push(`Led ${i}`);
	}

	return names;
};

export function DiscoveryService() {
	this.IconUrl = "https://raw.githubusercontent.com/qiangqiang101/SignalRGB-Wallpaper-Engine/refs/heads/main/srgbwallpaper.png";
	this.Initialize = () => {
		// service.addController(new Wallpaper({
		// 	id: "Wallpaper3",
		// 	port: 8132,
		// 	ip: "127.0.0.1",
		// 	name: "Wallpaper Engine 2 (debug)",
		// }));
		service.addController(new Wallpaper({
			id: "Wallpaper",
			port: 8133,
			ip: "127.0.0.1",
			name: "Wallpaper Engine 2",
		}));
		service.addController(new Wallpaper({
			id: "Wallpaper2",
			port: 8134,
			ip: "127.0.0.1",
			name: "Wallpaper Engine 2 (2nd Screen)",
		}));

		// const controller = service.getController("Wallpaper");
		// service.updateController(controller);
		// service.announceController(controller);

		const controllers = [service.getController("Wallpaper"), service.getController("Wallpaper2")];
		controllers.forEach(function (controller, index) {
			service.updateController(controller);
			service.announceController(controller);
		});

		updateSettings();
	};

	this.Update = () => {

		for (const cont of service.controllers) {
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

	update() {
		if (!this.initialized) {
			this.initialized = true;

			service.updateController(this);
			service.announceController(this);
		}
	}
}

export function ImageUrl() {
	return "https://raw.githubusercontent.com/qiangqiang101/SignalRGB-Wallpaper-Engine/refs/heads/main/srgbwallpaper.png";
}