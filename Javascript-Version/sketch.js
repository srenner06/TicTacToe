const VOID = 3;
const X = 1;
const O = -1;
const TIE = 0;
var board;
var turn;
var finished;
var buffer;
var w;

let round = 0;

function setup() {
  createCanvas(600, 700);
  textSize(30);
  w = width / 3;
  buffer = w / 4;
  finished = false;
  turn = X;
  //draw board
  fill(255);
  for (let i = 0; i < 3; i++) {
    for (let j = 0; j < 3; j++) {
      strokeWeight(1);
      rect(i * w, j * w, w, w);
    }
  }
  board = [
    [VOID, VOID, VOID],
    [VOID, VOID, VOID],
    [VOID, VOID, VOID]
  ];

  fill(255);
  strokeWeight(1);
  rect(0, 3 * w, 3 * w, w / 2);
  if(round%2==0) {
    let next = findBestTurn(board, turn, 0);
    turn = makeTurn(board, next.x, next.y, turn);
  }
}

function draw() {
  fill(255);
  strokeWeight(1);
  rect(0, 600, 600, w / 2);

  if (!finished) {
    if (turn == X) {
      fill(0);
      text("X ist dran", w, 3 * w + buffer);
    } else {
      fill(0);
      text("O ist dran", w, 3 * w + buffer);
    }
  }
  drawBoard(board);
  checkAndFinish(board);


}

function mouseReleased() {
  if (!finished) {
    var x = mouseX;
    var y = mouseY;
    noFill();
    //check board and draw corresponding shape on right field
    for (let i = 0; i < 3; i++) {
      for (let j = 0; j < 3; j++) {
        if (isbetween2(x, y, i * w, (i + 1) * w, j * w, (j + 1) * w) && board[i][j] == VOID) {
          turn = makeTurn(board, i, j, turn);
          if (checkAndFinish(board) == true) {
            return;
          }
          let next = findBestTurn(board, turn, 0);
          if (next.x != -1) {
            turn = makeTurn(board, next.x, next.y, turn);
          }
        }
      }
    }
  } else {
    finished = false;
    round++;
    setup();
  }
}

function makeTurn(board, x, y, player) {
  board[x][y] = player;
  return nextTurn(player);
}

function drawBoard(board) {
  for (let x = 0; x < board.length; x++) {
    for (let y = 0; y < board[0].length; y++) {
      let player = board[x][y];
      noFill();
      if (player == O) {
        strokeWeight(4);
        ellipse(x * w + w / 2, y * w + w / 2, w / 2, w / 2);
      } else if (player == X) {
        strokeWeight(4);
        cross(x * w + w / 2, y * w + w / 2, w - buffer);
      }
    }
  }
}

function nextTurn(turn) {
  if (turn == O) {
    return X;
  }
  return O;
}

function findBestTurn(b, player, count) { //find best Turn for player wiht minimax
  let board = copyArray(b);
  let max = player == X;
  let best = {
    x: -1,
    y: -1,
    sc: 2
  };
  if (max) {
    best.sc = -2;
  }
  let rows = board.length;
  let cols = board[0].length;

  for (let i = 0; i < rows; i++) {
    for (let j = 0; j < cols; j++) {
      if (board[i][j] == VOID) { //if no occupied => make that turn and score it
        board[i][j] = player;
        let curr = {
          x: i,
          y: j,
          sc: score(board, player, count) //score of the turn 1: X wins, -1: O wins, 0: tie
        };
        if (max) { //if solving for best turn for X: we want maximum turn (so that X wins)
          if (curr.sc > best.sc) {
            best = curr;
          }
        } else { //if solving for best turn for O: we want minimum turn (so that O wins)
          if (curr.sc < best.sc) {
            best = curr;
          }
        }
        board[i][j] = VOID;
      }
    }
  }

  return best;


}

function score(board, player, count) {
  let winner = checkWinner(board, false);
  if (winner != VOID) {
    return winner;
  }
  let next = findBestTurn(board, nextTurn(player), count + 1);
  return next.sc;
}



function checkWinner(board, draw) {
  //horizontal
  for (let j = 0; j < 3; j++) {
    if (areequal(board[0][j], board[1][j], board[2][j]) && board[0][j] != VOID) {
      if (draw) {
        strokeWeight(16);
        line(buffer, j * w + w / 2, 3 * w - buffer, j * w + w / 2);
      }
      return board[0][j];
    }
  }
  //vertical
  for (let i = 0; i < 3; i++) {
    if (areequal(board[i][0], board[i][1], board[i][2]) && board[i][0] != VOID) {
      if (draw) {
        strokeWeight(16);
        line(i * w + w / 2, buffer, i * w + w / 2, 3 * w - buffer);
      }
      return board[i][0];
    }
  }
  //diagonals
  if (areequal(board[0][0], board[1][1], board[2][2]) && board[0][0] != VOID) {
    if (draw) {
      strokeWeight(16);
      line(buffer, buffer, 3 * w - buffer, 3 * w - buffer);
    }
    return board[0][0];
  }
  if (areequal(board[2][0], board[1][1], board[0][2]) && board[2][0] != VOID) {
    if (draw) {
      strokeWeight(16);
      line(3 * w - buffer, buffer, buffer, 3 * w - buffer);
    }
    return board[2][0];
  }

  //go through every field and return no winner (VOID) if at least one field is still empty
  for (let i = 0; i < 3; i++) {
    for (let j = 0; j < 3; j++) {
      if (board[i][j] == VOID) {
        return VOID;
      }
    }
  }
  //else... (noone has three in a row, every field is obtained)
  return TIE;
}

function checkAndFinish(board) {
  let winner = checkWinner(board, true);
  if (winner != VOID) {
    finished = true;
    if (winner == X) {
      fill(0);
      text("X hat gewonnen", w, 650);
      //println(X);
    } else if (winner == O) {
      fill(0);
      text("O hat gewonnen", w, 650);
      //println(O);
    } else {
      fill(0);
      text("Unentschieden", w, 650);
      //println(TIE);
    }
  }
  return finished;
}

function copyArray(arr) {
  let cop = [];

  for (var i = 0; i < arr.length; i++) {
    cop[i] = arr[i].slice();
  }
  return cop;
}


function areequal(a, b, c) {
  return a == b && b == c;
}
//function drawing the cross
function cross(x, y, w) {
  line(x - w / 2, y - w / 2, x + w / 2, y + w / 2);
  line(x + w / 2, y - w / 2, x - w / 2, y + w / 2);
}

function isbetween(x, a, b) {
  return x >= a && x <= b;
}

function isbetween2(x, y, a, b, c, d) {
  return isbetween(x, a, b) && isbetween(y, c, d);
}