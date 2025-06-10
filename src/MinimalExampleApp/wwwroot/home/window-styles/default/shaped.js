(() => {
    const c = document.getElementById("canvas2");
    const g = c.getContext("2d");

    c.style.width = c.width / devicePixelRatio + "px";
    c.style.height = c.height / devicePixelRatio + "px";

    let t = 0;
    const colorAt = (y) => {
        const min = 45;
        const max = 70;
        //let's set the y start and end positions for this:
        const f = Math.max(0, Math.min(1, (y - min) / (max - min)));
        const i = 1 - f;

        return (
            "rgb(" +
            [
                [255, 254],
                [240, 86],
                [155, 180],
            ]
                .map((p) => {
                    const out = Math.round(p[0] * i + p[1] * f);
                    // console.log("out is ", out);
                    return out;
                })
                .join(",") +
            ")"
        );
    };

    setInterval(() => {
        c.width = c.width;
        g.translate(0, 100 * Math.sin(t++ / 10000));

        [170, 150, 120, 90, 60].forEach((r, i, a) => {
            g.beginPath();
            g.globalAlpha = Math.pow((i + 1) / a.length, 2);
            g.arc(150, 150, r / 2, 0, 7);
            g.fill();
        });
        g.globalCompositeOperation = "source-atop";
        g.globalAlpha = 1;
        g.scale(3, 3);
        for (var i = 0; i < 15; i++) {
            g.lineWidth = 13;
            const grad = g.createLinearGradient(0, i * 12 - 9, 0, i * 12 + 9);

            grad.addColorStop(0, colorAt(i * 12));
            grad.addColorStop(0.49, colorAt(i * 12 + 12));
            // grad.addColorStop(0.51,"green");
            grad.addColorStop(1, colorAt(i * 12 + 32));
            g.strokeStyle = grad;
            g.beginPath();

            for (var x = 0; x < 300; x += 5) {
                const env = (x / 100) * (1 - x / 100);
                g.lineTo(x, i * 12 + 20 * Math.cos(x / 8 + t / 3000) * Math.cos(t / 5000) * env);
            }
            g.stroke();
        }
    }, 16);
})();

const c = document.getElementById("canvas");
const g = c.getContext("2d");

c.style.width = c.width / devicePixelRatio + "px";
c.style.height = c.height / devicePixelRatio + "px";

Object.assign(document.querySelector(".canvas-outer").style, {
    width: (564 / devicePixelRatio).toFixed() + "px",
    height: (564 / devicePixelRatio).toFixed() + "px",
});
const omega = 590;

class Point {
    constructor(x, y) {
        this.x = x;
        this.y = y;
    }
    fromPolar(radius, angle) {
        this.x = radius * Math.cos(angle);
        this.y = radius * Math.sin(angle);
        return this;
    }
    add(p) {
        this.x += p.x;
        this.y += p.y;
        return this;
    }
    sub(p) {
        this.x -= p.x;
        this.y -= p.y;
        return this;
    }
    length() {
        return Math.sqrt(this.lengthSquared());
    }
    lengthSquared() {
        return this.x * this.x + this.y * this.y;
    }
    clone() {
        return new Point(this.x, this.y);
    }
}

const center = new Point(canvas.width / 2, canvas.height / 2);

const arc = (r, theta) => {
    r -= 6;
    g.arc(center.x, center.y, r, 0 - theta, Math.PI + theta);

    return { end: new Point().fromPolar(r, -theta).add(center), start: new Point().fromPolar(r, Math.PI + theta).add(center) };
};

const BORDER = "#d6f9ff";
let prev = [];

const update = () => {
    c.width = c.width;

    g.strokeStyle = BORDER;
    g.lineWidth = 20;
    g.beginPath();
    arc(279, Math.PI);
    g.stroke();

    g.save();
    clouds.forEach((cloud) => cloud.update());
    g.restore();

    let toRight = g.createLinearGradient(512, 200, 400, 400);
    toRight.addColorStop(1, "rgba(1,230,255,0)");
    toRight.addColorStop(0, "rgba(1,230,255,0.48)");

    const vGrag0 = g.createLinearGradient(0, 180, 0, 350);
    vGrag0.addColorStop(0, "rgb(0,218,230)");
    vGrag0.addColorStop(1, "rgb(0,139,255)");
    g.fillStyle = vGrag0;
    g.beginPath();
    let sides = arc(248, 0.0);
    let dx = sides.end.x - sides.start.x;
    let xo = 0;
    for (var x = sides.start.x; x < sides.end.x; xo++, x++) {
        const envelope = (xo / dx) * (1 - xo / dx) * 4;
        g.lineTo(x, sides.start.y + 65 * envelope * Math.cos(Date.now() / omega + (xo / dx) * 3));
    }
    g.fill();

    toRight = g.createLinearGradient(512, 200, 400, 400);
    toRight.addColorStop(0, "rgba(1,230,255,0.5)");
    toRight.addColorStop(1, "rgba(1,230,255,0.0)");

    g.fillStyle = toRight;
    g.beginPath();
    for (var x = sides.end.x; x > sides.start.x; xo--, x--) {
        const envelope = (xo / dx) * (1 - xo / dx) * 4;
        g.lineTo(x, sides.start.y + 65 * envelope * Math.cos(Date.now() / omega + (xo / dx) * 3));
    }

    for (var x = sides.start.x; x < sides.end.x; xo++, x++) {
        const envelope = (xo / dx) * (1 - xo / dx) * 4;
        g.lineTo(x, sides.start.y + (65 * envelope * Math.cos(Date.now() / omega + (xo / dx) * 3) + 55 * envelope));
    }
    g.fill();

    g.fillStyle = "rgb(62,224,237) ";
    g.beginPath();
    sides = arc(250, 0.0);
    dx = sides.end.x - sides.start.x;
    xo = 0;
    for (var x = sides.start.x; x < sides.end.x; xo++, x++) {
        const envelope = (xo / dx) * (1 - xo / dx) * 4;
        g.lineTo(x, sides.start.y + 65 * envelope * Math.sin(Date.now() / omega + (xo / dx) * 3));
    }
    // g.stroke();
    g.fill();

    const vGrad1 = g.createLinearGradient(0, 230, 0, 390);
    vGrad1.addColorStop(0, "rgb(0,218,230)");
    vGrad1.addColorStop(1, "rgb(0,139,255)");

    g.fillStyle = vGrad1;
    g.beginPath();
    sides = arc(248, -0.02);
    dx = sides.end.x - sides.start.x;
    xo = 0;
    for (var x = sides.start.x; x < sides.end.x; xo++, x++) {
        const envelope = (xo / dx) * (1 - xo / dx) * 4;
        g.lineTo(x, sides.start.y + 65 * envelope * Math.sin(Date.now() / omega + (xo / dx) * 3));
    }
    g.fill();

    toRight = g.createLinearGradient(512, 300, 400, 500);
    toRight.addColorStop(0, "rgba(1,230,255,0.5)");
    toRight.addColorStop(1, "rgba(1,230,255,0.0)");

    g.fillStyle = toRight;
    g.beginPath();
    for (var x = sides.end.x; x > sides.start.x; xo--, x--) {
        const envelope = (xo / dx) * (1 - xo / dx) * 4;
        g.lineTo(x, sides.start.y + 65 * envelope * Math.sin(Date.now() / omega + (xo / dx) * 3));
    }
    for (var x = sides.start.x; x < sides.end.x; xo++, x++) {
        const envelope = (xo / dx) * (1 - xo / dx) * 4;
        const sd = Math.sin(Date.now() / omega + (xo / dx) * 3);
        g.lineTo(x, sides.start.y + (65 * envelope * sd + 90 * envelope));
    }
    g.fill();

    const vGrad2 = g.createLinearGradient(0, 310, 0, 512);
    vGrad2.addColorStop(0, "rgb(100,218,230)");
    vGrad2.addColorStop(1, "rgb(0,139,255)");
    g.fillStyle = vGrad2;

    g.beginPath();
    sides = arc(248, -0.55);
    dx = sides.end.x - sides.start.x;
    xo = 0;
    for (var x = sides.start.x; x < sides.end.x; xo++, x++) {
        const envelope = (xo / dx) * (1 - xo / dx) * 4;
        g.lineTo(x, sides.start.y + 65 * envelope * Math.sin(Date.now() / omega + (xo / dx) * 3));
    }
    g.fill();

    g.lineWidth = 4;
    g.strokeStyle = "rgb(62,224,237)";
    g.fillStyle = toRight;
    g.beginPath();

    for (var x = sides.end.x; x > sides.start.x; xo--, x--) {
        const envelope = (xo / dx) * (1 - xo / dx) * 4;
        g.lineTo(x, sides.start.y + 65 * envelope * Math.sin(Date.now() / omega + (xo / dx) * 3));
    }
    g.stroke();
    let yo = [];
    for (var x = sides.start.x; x < sides.end.x; xo++, x++) {
        const envelope = (xo / dx) * (1 - xo / dx) * 4;
        const sd = Math.sin(Date.now() / omega + (xo / dx) * 3);

        const yy = sides.start.y + (65 * envelope * sd + 60 * envelope);
        g.lineTo(x, yy);
        yo[Math.floor(x)] = yy;
    }
    g.fill();

    //
    let dy = [];
    if (prev.length == yo.length) {
        dy = yo.map((e, i) => e - prev[i]);
    }
    g.globalCompositeOperation = "source-atop";
    g.globalAlpha = 0.6;
    // debugger;
    const bubbleGrad = g.createLinearGradient(0, -9, 0, 6);
    bubbleGrad.addColorStop(1, "rgba(0,240,254,0.6)");
    bubbleGrad.addColorStop(0, "rgba(0,240,254,0.0)");
    g.fillStyle = bubbleGrad;
    bubbles.forEach((b) => {
        circle(b.x, b.y, b.r);
        let yy = 0;
        let xx = Math.floor(b.x);
        if (dy[xx] !== undefined) yy = dy[xx] / 1.4;
        b.y -= b.r / 12 - yy;
        // b.r+=0.05;
        b.x -= (Math.cos(Date.now() / omega + (b.x / 512) * 3) * 2 * (512 - b.y)) / 1200 + 1;
        if (b.x < 0) b.x += 512 + Math.random() * 60;
        if (b.y < 218) (b.y += 312), (b.r = b.or);
    });
    // circle(100,100, 10);
    g.globalCompositeOperation = "source-atop";

    g.globalAlpha = 1;
    // g.strokeStyle = "#801010";
    g.strokeStyle = BORDER;
    g.lineWidth = 20;
    g.beginPath();
    arc(279, 0.3);
    g.stroke();
    prev = yo;

    requestAnimationFrame(update);
};
// debugger;

/*
there are a couple of directions I could move in for this, one is more
interesting horizontal bubble flow.

*/
class Cloud {
    constructor(d) {
        this.x = Math.random() * 0x400;
        this.y = 180 - Math.random() * 140;

        214, 249, 255;
        // const d =
        const r = (255 * d + 214 * (1 - d)).toFixed();
        const g = (255 * d + 249 * (1 - d)).toFixed();

        this.color = "rgb(" + r + "," + g + ",255)";

        this.velocity = (1 + d * 2) / 4;
        this.scale = 0.3 + d;

        const lDelta = Math.random() * 16 + 10;
        const rDelta = Math.random() * 16 + 10;
        this.boxHeight = Math.min(lDelta, rDelta);
        this.left = new Bubble({ r: lDelta, x: -30, y: -lDelta });
        this.right = new Bubble({ r: rDelta, x: 30, y: -rDelta });
        this.top = new Bubble({ r: 20, y: -30, x: 0 });
    }
    update() {
        this.x += this.velocity;
        if (this.x > 650) this.x -= 750;
        this.draw();
    }
    draw() {
        g.translate(this.x, this.y + 10);
        g.scale(this.scale, this.scale);
        g.fillStyle = BORDER;
        circle(this.left);
        circle(this.right);
        circle(this.top);
        g.fillRect(-30, -this.boxHeight * 2, 60, this.boxHeight * 2);
        g.translate(-this.x, -this.y - 10);

        g.translate(this.x, this.y);
        g.fillStyle = this.color;

        circle(this.left);
        circle(this.right);
        circle(this.top);
        g.fillRect(-30, -this.boxHeight * 2, 60, this.boxHeight * 2);
        g.scale(1 / this.scale, 1 / this.scale);
        g.translate(-this.x, -this.y);
    }
}

class Bubble {
    constructor(params) {
        {
            this.x = Math.random() * 0x200;
            this.y = Math.random() * 0x200;
            this.or = Math.random() * 5 + 5;
            this.r = Math.random() * 5 + 5;
            this.vy = Math.random() * 3 + 1;
            if (params) Object.assign(this, params);
        }
    }
}

const bubbles = new Array(30).fill(0).map((e, i) => new Bubble());

const circle = (o, y, r) => {
    if (o.y != undefined) {
        circle(o.x, o.y, o.r);
        return;
    }
    let x = o;
    g.translate(x, y);
    g.beginPath();
    g.arc(0, 0, r, 0, 7);
    g.fill();
    g.translate(-x, -y);
};

const clouds = new Array(10).fill(0).map((_, i, a) => {
    return new Cloud(i / a.length);
});

requestAnimationFrame(update);
