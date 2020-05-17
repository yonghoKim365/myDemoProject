package component;
/*
** ####    ###    ###   ####   ###
** #   #  #   #  #   #  #   #  #  #
** ####   #   #  #####  ####   #  #
** #   #  #   #  #   #  # #    #  #
** ####    ###   #   #  #  ##  ###
**
**    - 우선순위 5영역-
**
** ① ⑤ ② ② ② ② ⑤ ①
** ⑤ ⑤ ④ ④ ④ ④ ⑤ ⑤
** ② ④ ③ ③ ③ ③ ④ ②
** ② ④ ③ ③ ③ ③ ④ ②
** ② ④ ③ ③ ③ ③ ④ ②
** ② ④ ③ ③ ③ ③ ④ ②
** ⑤ ⑤ ④ ④ ④ ④ ⑤ ⑤
** ① ⑤ ② ② ② ② ⑤ ①
*/



public class Board {
    public final static int EMPTY = 0;              // 빈 공간
    public final static int WHITE_STONE = 1;     // 흰돌
    public final static int BLACK_STONE = 2;        // 검은돌
    public final static int ACTIVE_STONE = 3;       // 바뀌어야하는 돌

    public final static int WIDTH = 8;              // 게임판 가로 크기 (단위: 칸)
    public final static int HEIGHT = 8;             // 게임판 세로 크기 (단위: 칸)

    //private int board[][] = new int[HEIGHT][WIDTH];
    public int board[][] = new int[HEIGHT][WIDTH];

    // 마지막으로 둔 돌의 좌표
    private int lastX = 0;
    private int lastY = 0;

    // 5단계의 우선 순위 x 좌표
    private int[][] cx = {
        {0, 7, 0, 7},
        {2, 3, 4, 5, 2, 3, 4, 5, 0, 0, 0, 0, 7, 7, 7, 7},
        {2, 3, 4, 5, 2, 3, 4, 5, 2, 3, 4, 5, 2, 3, 4, 5},
        {2, 3, 4, 5, 2, 3, 4, 5, 1, 1, 1, 1, 6, 6, 6, 6},
        {1, 0, 1, 6, 7, 6, 0, 1, 1, 6, 6, 7}
    };

    // 5단계의 우선 순위 y 좌표
    private int[][] cy = {
        {0, 7, 7, 0},
        {0, 0, 0, 0, 7, 7, 7, 7, 2, 3, 4, 5, 2, 3, 4, 5},
        {2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5},
        {1, 1, 1, 1, 6, 6, 6, 6, 2, 3, 4, 5, 2, 3, 4, 5},
        {0, 1, 1, 6, 6, 7, 6, 6, 7, 0, 1, 1}
    };

    	public Board() {
        	initBoard(false);
    	}

    	public void dispose(){
    		board = null;
	}

/*
** ----------------------------------------
** 게임판을 초기화
** ----------------------------------------
*/
    public void initBoard(boolean test) {
        for (int i=0; i<HEIGHT; i++) {
            for (int j=0; j<WIDTH; j++) {
                board[i][j] = EMPTY;
            }
        }

        // ○●
        // ●○
        board[3][3] = WHITE_STONE;
        board[4][4] = WHITE_STONE;
        board[3][4] = BLACK_STONE;
        board[4][3] = BLACK_STONE;

	/*
	///////////////////////////////////////////////////// for test
	if (test){

		int win_stone = BLACK_STONE;//WHITE_STONE;
		int lose_stone = WHITE_STONE; //BLACK_STONE;
	        // for test
		for (int y=0;y<8;y++){
			for (int x=0;x<8;x++){
				board[y][x] = win_stone;
			}
		}

		board[0][0] = lose_stone;
		board[0][6] = EMPTY;
		board[0][7] = EMPTY;

		for (int x=0;x<8;x++){
			board[1][x] = lose_stone;
		}

		board[1][0] = win_stone;
		board[1][7] = EMPTY;

		board[7][0] = lose_stone;

		board[3][7] = EMPTY;

		//board[7][0] = EMPTY;
		//board[7][0] = EMPTY;

		board[7][4] = EMPTY;
		board[7][5] = EMPTY;
		board[7][6] = EMPTY;
		board[7][7] = EMPTY;

		board[4][7] = EMPTY;
		board[5][7] = EMPTY;
		//board[6][7] = EMPTY;
		board[7][7] = EMPTY;

		for (int x=0;x<4;x++){

			board[5][x] = lose_stone;
		}
	}
	*/

    }

/*
** ----------------------------------------
** 게임판에서 (x, y) 위치의 돌을 리턴한다.
** ----------------------------------------
*/
    public int getStone(int x, int y) {
        return (x<0 || x>WIDTH-1 || y<0 || y>HEIGHT-1) ? -1 : board[y][x];
    }

/*
** ----------------------------------------
** Active 상태의 돌을 뒤집힌 돌로 바꾼다
**
** 입력: sType = 바뀔 돌 종류(sType이
**               HUMAN_STONE 이면 ACTIVE_STONE 들은
**               HUMAN_STONE으로 바뀐다)
** ----------------------------------------
*/
    public void changeActiveStone(int sType)
    {
        for (int i=0; i<HEIGHT; i++)
            for (int j=0; j<WIDTH; j++)
                if (board[i][j] == ACTIVE_STONE) board[i][j] = sType;
    }

/*
** ----------------------------------------
** putFlag의 값에 따라, (x, y)에 돌을 놓을
** 수 있는지 검사하거나 돌을 놓는다
**
** x : 돌을 놓을 x 좌표
** y : 돌을 놓을 y 좌표
** stoneType : 놓는 돌의 종류
** putFlag : true면 돌을 놓은다음 상대편 돌을
**           뒤집고, false면 돌을 놓을 수
**           있는지의 여부만 검사한다
**
** 리턴값 :
**
** retValue : 돌을 놓았거나, 돌을 놓을 수
**            있으면 true 리턴. 돌을 놓을 수
**            없으면 false 리턴
** ----------------------------------------
*/
    public boolean putStone(int x, int y, int sType, boolean putFlag) {
        int stepCount = 0;          // 뒤집을 수 있는 돌의 수
        int i, j;                   // 현재 방향의 다음 돌의 위치
        int otherType;              // 뒤집힐 대상이 되는 돌
        boolean retValue = false;   // 돌을 놓는데 성공했는지의 여부를 나타냄

        // 빈 자리가 아니면 false 리턴
        if (getStone(x, y) != EMPTY) return false;

        // 뒤집힐 대상이 되는 돌을 알아낸다.
        otherType = (sType == BLACK_STONE) ? WHITE_STONE : BLACK_STONE;

        // 둘러쌀 수 있는 돌이 있는지 8방향을 검사한다
        for (int xdir=-1; xdir<2; xdir++)
        {
        	for (int ydir=-1; ydir<2; ydir++)
        	{
        		stepCount = 0;

        		// 둘러쌀 수 있는 돌의 갯수 계산
        		do {
        			stepCount++;
        			i = x + stepCount * xdir;
        			j = y + stepCount * ydir;
        		}
        		while (i>=0 && i<8 && j>=0 && j<8 && getStone(i,j)==otherType);

        		// 뒤집을 수 있는 돌이 있으면 true
        		if (i>=0 && i<8 && j>=0 && j<8 && stepCount>1 && getStone(i,j)==sType)
        		{
        			retValue = true;
        			// putFlag가 true면 돌을 뒤집는다
        			if (putFlag) {
        				for (int k=0; k<stepCount; k++)
        					board[y+ydir*k][x+xdir*k] = ACTIVE_STONE;
        				//System.out.println("[dreamer] ACTIVE_STONE " + ACTIVE_STONE);
        			}
        		}
        	}
        } // for

        // 뒤집히는 돌이 있으면 바뀌는 모양이 에니메이션으로 구현이 되는데
        // 돌을 놓은 자리는 에니메이션 처리가 되지 않도록 하기위해서
        // ACTIVE_STONE이 아닌 일반 돌을 놓는다
        if (putFlag==true && retValue==true) { board[y][x] = sType; lastX = x; lastY = y; }
        return retValue;
    }

/*
** ----------------------------------------
** 컴퓨터가 돌을 두는 함수. 유리한 위치부터
** 돌을 놓을 수 있는지 차례대로 검사해서
** 돌을 놓는다
** ----------------------------------------
*/
    public boolean putComputer() {
        // 검사할 영역이 우선순위에 따라 5군데 이므로 5번 반복
        for (int i=0; i<cx.length; i++) {
            shuffleCoordinate(cx[i], cy[i], cx[i].length);
            for (int j=0; j<cx[i].length; j++) {
                if (putStone(cx[i][j], cy[i][j], WHITE_STONE, true)) return true;
            }
        }
        // 돌을 놓을 자리가 없으면 false 리턴
        return false;
    }
/*
** ----------------------------------------
** 돌을 둘 자리가 있는지 모든 칸을 검사한다
**
**      리턴값: true=있다, false=없다
** ----------------------------------------
*/
    public boolean check_DOOL_JA_RI(int sType) {
        for (int y=0; y<HEIGHT; y++) {
            for (int x=0; x<WIDTH; x++) {
                if (putStone(x, y, sType, false)) return true;
            }
        }
        return false;
    }

/*
** ----------------------------------------
** ACTIVE_STONE 이 있는지 검사하고 갯수를 세어서 리턴
** ----------------------------------------
*/
    public int isActiveStone() {
    	int cntActiveStones = 0;
    	for (int i=0; i<HEIGHT; i++)
    		for (int j=0; j<WIDTH; j++)
    			if (board[i][j] == ACTIVE_STONE)
    			{
    				cntActiveStones++;
    			}

    	return cntActiveStones;
    }

/*
** ----------------------------------------
** 돌을 놓는 순서가 항상 일정하지 않도록
** 셀 좌표를 뒤섞는다.
** ----------------------------------------
*/

    public void shuffleCoordinate(int[] xArray, int[] yArray, int arrayLength) {
        int idx1;
        int idx2;
        int tempX;
        int tempY;

        for (int i=0; i<arrayLength; i++) {
            idx1 = (int)(Math.random() * arrayLength);
            idx2 = (int)(Math.random() * arrayLength);
            tempX = xArray[idx1];
            tempY = yArray[idx1];
            xArray[idx1] = xArray[idx2];
            yArray[idx1] = yArray[idx2];
            xArray[idx2] = tempX;
            yArray[idx2] = tempY;
        }
    }

/*
** ----------------------------------------
** 마지막으로 놓은 돌의 X 좌표 리턴
** ----------------------------------------
*/

    public int getLastX() {
        return lastX;
    }

/*
** ----------------------------------------
** 마지막으로 놓은 돌의 Y 좌표 리턴
** ----------------------------------------
*/

    public int getLastY() {
        return lastY;
    }
}
