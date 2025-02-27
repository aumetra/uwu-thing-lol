# Simple Pong game
# src: https://github.com/K-G-PRAJWAL/Python-Projects/blob/master/Pong/Pong.py

import turtle  # Basic Graphics module
# import winsound  # For Sound

import requests
import json
import time


SERVER = "http://151.217.2.77:5000/PingPong"
REQUEST_EACH = 10
request_counter = 0

wn = turtle.Screen()
wn.title("Pong by K G Prajwal")  # Window title
wn.bgcolor("black")  # Window background
wn.setup(width=1920, height=1080)  # Window size
wn.tracer(0)  # Stops window from updating - Speedup

windowHalfWidth = 1920 / 2
windowHalfHeight = 1080 / 2

# Scoreboard
score_a = 0
score_b = 0

# Paddle A
paddle_a = turtle.Turtle()
paddle_a.speed(0)  # Speed of animation
paddle_a.shape("square")  # Set shape as square (default 20x20)
paddle_a.color("white")  # Set the color
# Make the square into rectangle
paddle_a.shapesize(stretch_wid=5, stretch_len=1)
paddle_a.penup()  # Don't draw continously
paddle_a.goto(-350, 0)  # Paddle a starts at 350, left of screen

# Paddle B
paddle_b = turtle.Turtle()
paddle_b.speed(0)
paddle_b.shape("square")
paddle_b.color("white")
paddle_b.shapesize(stretch_wid=5, stretch_len=1)
paddle_b.penup()
paddle_b.goto(350, 0)  # Paddle a starts at 350, right of screen


# Ball
ball = turtle.Turtle()
ball.speed(0)
ball.shape("square")
ball.color("white")
ball.penup()
ball.goto(0, 0)
ball.dx = 1.5
ball.dy = 1.5

# Pen - Scoreboard
pen = turtle.Turtle()
pen.speed(0)
pen.color('white')
pen.penup()
pen.hideturtle()
pen.goto(0, 260)
pen.write("Player A: 0  Player B: 0", align="center",
          font=("Courier", 24, "normal"))


# Functionality
def paddle_a_up():
    y = paddle_a.ycor()  # .ycor() return y coordinate
    y += 20  # Move up
    paddle_a.sety(y)


def paddle_a_down():
    y = paddle_a.ycor()
    y -= 20  # Move down
    paddle_a.sety(y)


def paddle_b_up():
    y = paddle_b.ycor()
    y += 20
    paddle_b.sety(y)


def paddle_b_down():
    y = paddle_b.ycor()
    y -= 20
    paddle_b.sety(y)


def build_json():
    data = {
        "player1X": round(paddle_a.xcor() + windowHalfWidth),
        "player1Y": round(-paddle_a.ycor() + windowHalfHeight),
        "player2X": round(paddle_b.xcor() + windowHalfWidth),
        "player2Y": round(-paddle_b.ycor() + windowHalfHeight),
        "ballX": round(ball.xcor() + windowHalfWidth),
        "ballY": round(-ball.ycor() + windowHalfHeight)
    }
    return data

def send_data():
    headers = {'Content-Type': 'application/json'}
    
    data = build_json()
    print(f'{data=}')
    res = requests.post(SERVER, json=data)
    print(res)

# Keyboard binding
wn.listen()
wn.onkeypress(paddle_a_up, "w")
wn.onkeypress(paddle_a_down, "s")
wn.onkeypress(paddle_b_up, "Up")
wn.onkeypress(paddle_b_down, "Down")

# Main game loop
while True:
    time.sleep(0.001)  # Delay for game

    # handle keys:
    # if wn.on

    wn.update()  # Update screen everytime loop runs

    # Move the ball
    ball.setx(ball.xcor() + ball.dx)
    ball.sety(ball.ycor() + ball.dy)

    # Top & Bottom Border
    if ball.ycor() > 290:
        ball.sety(290)
        ball.dy *= -1  # Reverse the direction of ball
        # winsound.PlaySound("bounce.wav", winsound.SND_ASYNC)

    if ball.ycor() < -290:
        ball.sety(-290)
        ball.dy *= -1
        # winsound.PlaySound("bounce.wav", winsound.SND_ASYNC)

    # Left & Right Border
    if ball.xcor() > 390:
        score_a += 1
        pen.clear()
        pen.write("Player A: {}  Player B: {}".format(score_a, score_b), align="center",
                  font=("Courier", 24, "normal"))  # Update score
        ball.goto(0, 0)
        ball.dx *= -1

    if ball.xcor() < -390:
        score_b += 1
        pen.clear()
        pen.write("Player A: {}  Player B: {}".format(score_a, score_b), align="center",
                  font=("Courier", 24, "normal"))
        ball.goto(0, 0)
        ball.dx *= -1

    # Bounce of the paddle
    if (ball.xcor() > 340 and ball.xcor() < 350) and (ball.ycor() < paddle_b.ycor() + 40 and ball.ycor() > paddle_b.ycor() - 50):
        ball.setx(340)
        ball.dx *= -1
        # winsound.PlaySound("bounce.wav", winsound.SND_ASYNC)

    if (ball.xcor() < -340 and ball.xcor() > -350) and (ball.ycor() < paddle_a.ycor() + 40 and ball.ycor() > paddle_a.ycor() - 50):
        ball.setx(-340)
        ball.dx *= -1
        # winsound.PlaySound("bounce.wav", winsound.SND_ASYNC)

    if(request_counter > REQUEST_EACH):
        # build_json()
        send_data()
        request_counter = 0
    else:
        request_counter = request_counter + 1