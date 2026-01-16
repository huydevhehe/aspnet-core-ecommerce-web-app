document.addEventListener("DOMContentLoaded", function () {
    const canvas = document.getElementById("flappyCanvas");
    const ctx = canvas.getContext("2d");

    canvas.width = 400;
    canvas.height = 500;

    const bird = {
        x: 50,
        y: 150,
        width: 30,
        height: 30,
        gravity: 0.4,  // Gi?m tr?ng l?c ð? rõi mý?t hõn
        lift: -7.5,     // Gi?m l?c nh?y ð? t? nhiên hõn
        velocity: 0
    };

    const pipes = [];
    const pipeWidth = 60;
    const pipeGap = 140;
    let frame = 0;
    let score = 0;
    let gameRunning = false;

    function drawBird() {
        ctx.fillStyle = "yellow";
        ctx.beginPath();
        ctx.arc(bird.x + bird.width / 2, bird.y + bird.height / 2, bird.width / 2, 0, Math.PI * 2);
        ctx.fill();
        ctx.closePath();
    }

    function drawPipes() {
        ctx.fillStyle = "green";
        pipes.forEach(pipe => {
            ctx.fillRect(pipe.x, 0, pipeWidth, pipe.top);
            ctx.fillRect(pipe.x, pipe.top + pipeGap, pipeWidth, canvas.height - pipe.top - pipeGap);
        });
    }

    function updateGame() {
        if (!gameRunning) return;

        bird.velocity += bird.gravity;
        bird.velocity *= 0.9;  // Gi?m t?c ð? rõi giúp mý?t hõn
        bird.y += bird.velocity;

        if (bird.y + bird.height >= canvas.height || bird.y <= 0) {
            resetGame();
        }

        if (frame % 90 === 0) {
            const pipeHeight = Math.random() * (canvas.height / 2) + 50;
            pipes.push({ x: canvas.width, top: pipeHeight });
        }

        pipes.forEach((pipe, index) => {
            pipe.x -= 3;

            if (pipe.x + pipeWidth < 0) {
                pipes.splice(index, 1);
                score++;
            }

            if (
                bird.x < pipe.x + pipeWidth &&
                bird.x + bird.width > pipe.x &&
                (bird.y < pipe.top || bird.y + bird.height > pipe.top + pipeGap)
            ) {
                resetGame();
            }
        });

        frame++;
    }

    function resetGame() {
        bird.y = 150;
        bird.velocity = 0;
        pipes.length = 0;
        frame = 0;
        score = 0;
        gameRunning = false;
    }

    function gameLoop() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        drawBird();
        drawPipes();
        updateGame();
        requestAnimationFrame(gameLoop);
    }

    // ?? Chuy?n ði?u khi?n t? phím Space sang Click chu?t
    document.addEventListener("click", function () {
        if (!gameRunning) {
            gameRunning = true;
        }
        bird.velocity = bird.lift;
    });

    gameLoop();
});
