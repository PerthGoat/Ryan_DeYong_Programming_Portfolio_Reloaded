var canvas = document.getElementById("theCanvas");

var ctx = canvas.getContext("2d");

var canvas_rect = canvas.getBoundingClientRect();

var letter_box = document.getElementById("the_letter");

var x0 = -1;
var y0 = -1;

var dsumx = 0;
var dsumy = 0;
var sums = 0;

var mouseDown = 0;

var stroke = "";
var strokes = [];

var patterns = [
["LEFT", "DOWN", "RIGHT", "UP"], // number 0
["DOWN"], // number 1
["UP"], // number 1 ALTERNATIVE DRAWING METHOD
["RIGHT", "DOWN", "LEFT", "RIGHT"], // number 2
["UP", "RIGHT", "DOWN", "LEFT", "RIGHT"], // number 2 alternative
["RIGHT", "DOWN", "LEFT", "RIGHT", "DOWN", "LEFT"], // number 3
["RIGHT", "DOWN", "LEFT", "DOWN", "LEFT"], // number 3 ALTERNATIVE
["UP", "RIGHT", "DOWN", "LEFT", "DOWN", "LEFT"], // number 3 alternative 2
["UP", "RIGHT", "DOWN", "LEFT", "RIGHT", "DOWN", "LEFT"], // number 3 alternative 3
["RIGHT", "DOWN", "LEFT", "DOWN", "RIGHT", "DOWN", "LEFT"], // number 3 alternative 4
["DOWN", "RIGHT", "UP", "DOWN"], // number 4
["LEFT", "DOWN", "RIGHT", "DOWN", "LEFT"], // number 5
["LEFT", "DOWN", "RIGHT", "UP", "LEFT"], // number 6
["RIGHT", "DOWN"], // number 7
["LEFT", "DOWN", "RIGHT", "DOWN", "LEFT", "UP", "RIGHT", "UP"], // number 8
["LEFT", "DOWN", "RIGHT", "DOWN", "LEFT", "UP"], // 8 alternative
["LEFT", "DOWN", "LEFT", "UP", "RIGHT", "UP"], // 8 alternative 2
["LEFT", "DOWN", "RIGHT", "DOWN", "LEFT", "UP", "RIGHT"], // 8 alternative 3
["UP", "LEFT", "DOWN", "RIGHT"], // number 9
["LEFT", "DOWN", "RIGHT", "UP", "DOWN"] // number 9 ALTERNATIVE DRAWING METHOD
];

var vals = [0, 1, 1, 2, 2, 3, 3, 3, 3, 3, 4, 5, 6, 7, 8, 8, 8, 8, 9, 9];

function clear_all() {
	ctx.clearRect(0, 0, canvas.width, canvas.height);
	letter_box.innerHTML = "Nothing";
}

function recordStroke(s) {
	if(stroke != s) {
		stroke = s;
		strokes.push(s);
	}
}

function tryMatchChar() {
	
	for(var i = 0;i < patterns.length;i++) {
		if(patterns[i].length === strokes.length && patterns[i].every(function(v,i) { return v === strokes[i]})) {
			letter_box.innerHTML = vals[i];
			//console.log(vals[i]);
			break;
		}
	}
	
	//console.log(strokes);
	
	strokes = [];
}

canvas.addEventListener('mousedown', function(e) {
	mouseDown = 1;
});

canvas.addEventListener('mouseup', function(e) {
	mouseDown = 0;
	x0 = -1;
	y0 = -1;
	dsumx = 0;
	dsumy = 0;
	sums = 0;
	stroke = "";
	tryMatchChar();
});

canvas.addEventListener('mousemove', function(e) {

	if(mouseDown == 1) {
		
		if(x0 != -1 && y0 != -1) {
			
			var dx = x0 - e.clientX;
			var dy = y0 - e.clientY;
			
			dsumx += dx;
			dsumy += dy;
			sums++;
			
			ctx.beginPath();
			ctx.moveTo(x0 - canvas_rect.left, y0 - canvas_rect.top);
			x0 = e.clientX;
			y0 = e.clientY;
			ctx.lineTo(x0 - canvas_rect.left, y0 - canvas_rect.top);
			ctx.stroke();
			
			if(Math.abs(dsumx) + Math.abs(dsumy) > 10) {
				if(Math.abs(dsumx / sums) > Math.abs(dsumy / sums)) {
					if((dsumx / sums) < 0) {
						recordStroke("RIGHT");
					} else {
						recordStroke("LEFT");
					}
				} else {
					if((dsumy / sums) < 0) {
						recordStroke("DOWN");
					} else {
						recordStroke("UP");
					}
				}
			
				dsumx = 0;
				dsumy = 0;
				sums = 0;
			}
		
		} else {
			x0 = e.clientX;
			y0 = e.clientY;
		}
	}
});