const canvas = document.getElementById("c1");
const c = canvas.getContext("2d");
canvas.width = window.innerWidth;
canvas.height = window.innerHeight;
//Falling Text
class Text {
    constructor(x, y, l, tip) {
        //Where Text is on Grid
        this.x = x;
        this.y = y;
        //Visual Features
        this.l = l;
        this.tip = tip;
        this.b = 55;
        let r = Math.random();
        if (r < 0.6) this.val = String.fromCharCode(0x30a0 + Math.round(Math.random() * 96));
        else if (r < 0.9) this.val = String.fromCharCode(0x50d0 + Math.round(Math.random() * 222));
        else this.val = Math.round(Math.random() * 9);
    }
    update() {
        if (this.b > 55) {
            //Changing Character
            if (Math.random() < 0.1) {
                let r = Math.random();
                if (r < 0.6) this.val = String.fromCharCode(0x30a0 + Math.round(Math.random() * 96));
                else if (r < 0.9) this.val = String.fromCharCode(0x50d0 + Math.round(Math.random() * 222));
                else this.val = Math.round(Math.random() * 9);
            }
            //Fading Text Out
            this.b -= this.l;
        } else this.b = 55;
    }
    show() {
        //Shading Text
        if (this.tip && this.b == 255) c.fillStyle = "rgb(200, 255, 200)";
        else c.fillStyle = "rgb(0, " + this.b + ", 0)";
        c.fillText(this.val, this.x, this.y);
    }
}
//Streaks Of Text
class Streak {
    constructor(x) {
        //Array Holding Text Objects Belonging to This Streak
        this.t = [];
        let l = Math.random() * 10 + 5;
        let tip;
        if (Math.random() < 0.4) tip = true;
        else tip = false;
        for (let i = 0; i < rows; i++) {
            this.t[i] = new Text(x * inc + inc / 2, i * inc + inc, l, tip);
        }
        //Timing Data
        this.del = Math.random() * 140 + 10;
        this.time = 0;
        this.done = false;
        this.interval = Math.round(Math.random() * 2 + 1);
        this.count = 0;
    }
    run() {
        //When To Start Streak
        if (!this.done) {
            if (this.time < this.del) this.time++;
            else {
                this.t[0].b = 255;
                this.done = true;
            }
        }
        //Displaying Characters
        for (let i = rows - 1; i >= 0; i--) {
            this.t[i].show();
        }
        //When To Update Characters
        this.count++;
        if (this.count > this.interval) {
            this.count = 0;
            for (let i = rows - 1; i >= 0; i--) {
                //Moving Streaks Down Screen
                if (this.t[i].b == 255) {
                    if (i < rows - 1) this.t[i + 1].b = 255;
                    else this.t[0].b = 255;
                }
                this.t[i].update();
            }
        }
    }
}
let inc = 32;
let cols = Math.ceil(canvas.width / inc);
let rows = Math.ceil(canvas.height / inc);
//Creating Streaks
let s = [];
for (let i = 0; i < cols; i++) s[i] = new Streak(i);
c.font = inc + "px Arial";
c.textAlign = "center";
//Animation Loop
function draw() {
    requestAnimationFrame(draw);
    c.clearRect(0, 0, canvas.width, canvas.height);
    //Running Streaks
    for (let i = 0; i < cols; i++) s[i].run();
}
draw();
