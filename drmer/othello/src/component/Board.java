package component;
/*
** ####    ###    ###   ####   ###
** #   #  #   #  #   #  #   #  #  #
** ####   #   #  #####  ####   #  #
** #   #  #   #  #   #  # #    #  #
** ####    ###   #   #  #  ##  ###
**
**    - �켱���� 5����-
**
** �� �� �� �� �� �� �� ��
** �� �� �� �� �� �� �� ��
** �� �� �� �� �� �� �� ��
** �� �� �� �� �� �� �� ��
** �� �� �� �� �� �� �� ��
** �� �� �� �� �� �� �� ��
** �� �� �� �� �� �� �� ��
** �� �� �� �� �� �� �� ��
*/



public class Board {
    public final static int EMPTY = 0;              // �� ����
    public final static int WHITE_STONE = 1;     // ��
    public final static int BLACK_STONE = 2;        // ������
    public final static int ACTIVE_STONE = 3;       // �ٲ����ϴ� ��

    public final static int WIDTH = 8;              // ������ ���� ũ�� (����: ĭ)
    public final static int HEIGHT = 8;             // ������ ���� ũ�� (����: ĭ)

    //private int board[][] = new int[HEIGHT][WIDTH];
    public int board[][] = new int[HEIGHT][WIDTH];

    // ���������� �� ���� ��ǥ
    private int lastX = 0;
    private int lastY = 0;

    // 5�ܰ��� �켱 ���� x ��ǥ
    private int[][] cx = {
        {0, 7, 0, 7},
        {2, 3, 4, 5, 2, 3, 4, 5, 0, 0, 0, 0, 7, 7, 7, 7},
        {2, 3, 4, 5, 2, 3, 4, 5, 2, 3, 4, 5, 2, 3, 4, 5},
        {2, 3, 4, 5, 2, 3, 4, 5, 1, 1, 1, 1, 6, 6, 6, 6},
        {1, 0, 1, 6, 7, 6, 0, 1, 1, 6, 6, 7}
    };

    // 5�ܰ��� �켱 ���� y ��ǥ
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
** �������� �ʱ�ȭ
** ----------------------------------------
*/
    public void initBoard(boolean test) {
        for (int i=0; i<HEIGHT; i++) {
            for (int j=0; j<WIDTH; j++) {
                board[i][j] = EMPTY;
            }
        }

        // �ۡ�
        // �ܡ�
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
** �����ǿ��� (x, y) ��ġ�� ���� �����Ѵ�.
** ----------------------------------------
*/
    public int getStone(int x, int y) {
        return (x<0 || x>WIDTH-1 || y<0 || y>HEIGHT-1) ? -1 : board[y][x];
    }

/*
** ----------------------------------------
** Active ������ ���� ������ ���� �ٲ۴�
**
** �Է�: sType = �ٲ� �� ����(sType��
**               HUMAN_STONE �̸� ACTIVE_STONE ����
**               HUMAN_STONE���� �ٲ��)
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
** putFlag�� ���� ����, (x, y)�� ���� ����
** �� �ִ��� �˻��ϰų� ���� ���´�
**
** x : ���� ���� x ��ǥ
** y : ���� ���� y ��ǥ
** stoneType : ���� ���� ����
** putFlag : true�� ���� �������� ����� ����
**           ������, false�� ���� ���� ��
**           �ִ����� ���θ� �˻��Ѵ�
**
** ���ϰ� :
**
** retValue : ���� ���Ұų�, ���� ���� ��
**            ������ true ����. ���� ���� ��
**            ������ false ����
** ----------------------------------------
*/
    public boolean putStone(int x, int y, int sType, boolean putFlag) {
        int stepCount = 0;          // ������ �� �ִ� ���� ��
        int i, j;                   // ���� ������ ���� ���� ��ġ
        int otherType;              // ������ ����� �Ǵ� ��
        boolean retValue = false;   // ���� ���µ� �����ߴ����� ���θ� ��Ÿ��

        // �� �ڸ��� �ƴϸ� false ����
        if (getStone(x, y) != EMPTY) return false;

        // ������ ����� �Ǵ� ���� �˾Ƴ���.
        otherType = (sType == BLACK_STONE) ? WHITE_STONE : BLACK_STONE;

        // �ѷ��� �� �ִ� ���� �ִ��� 8������ �˻��Ѵ�
        for (int xdir=-1; xdir<2; xdir++)
        {
        	for (int ydir=-1; ydir<2; ydir++)
        	{
        		stepCount = 0;

        		// �ѷ��� �� �ִ� ���� ���� ���
        		do {
        			stepCount++;
        			i = x + stepCount * xdir;
        			j = y + stepCount * ydir;
        		}
        		while (i>=0 && i<8 && j>=0 && j<8 && getStone(i,j)==otherType);

        		// ������ �� �ִ� ���� ������ true
        		if (i>=0 && i<8 && j>=0 && j<8 && stepCount>1 && getStone(i,j)==sType)
        		{
        			retValue = true;
        			// putFlag�� true�� ���� �����´�
        			if (putFlag) {
        				for (int k=0; k<stepCount; k++)
        					board[y+ydir*k][x+xdir*k] = ACTIVE_STONE;
        				//System.out.println("[dreamer] ACTIVE_STONE " + ACTIVE_STONE);
        			}
        		}
        	}
        } // for

        // �������� ���� ������ �ٲ�� ����� ���ϸ��̼����� ������ �Ǵµ�
        // ���� ���� �ڸ��� ���ϸ��̼� ó���� ���� �ʵ��� �ϱ����ؼ�
        // ACTIVE_STONE�� �ƴ� �Ϲ� ���� ���´�
        if (putFlag==true && retValue==true) { board[y][x] = sType; lastX = x; lastY = y; }
        return retValue;
    }

/*
** ----------------------------------------
** ��ǻ�Ͱ� ���� �δ� �Լ�. ������ ��ġ����
** ���� ���� �� �ִ��� ���ʴ�� �˻��ؼ�
** ���� ���´�
** ----------------------------------------
*/
    public boolean putComputer() {
        // �˻��� ������ �켱������ ���� 5���� �̹Ƿ� 5�� �ݺ�
        for (int i=0; i<cx.length; i++) {
            shuffleCoordinate(cx[i], cy[i], cx[i].length);
            for (int j=0; j<cx[i].length; j++) {
                if (putStone(cx[i][j], cy[i][j], WHITE_STONE, true)) return true;
            }
        }
        // ���� ���� �ڸ��� ������ false ����
        return false;
    }
/*
** ----------------------------------------
** ���� �� �ڸ��� �ִ��� ��� ĭ�� �˻��Ѵ�
**
**      ���ϰ�: true=�ִ�, false=����
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
** ACTIVE_STONE �� �ִ��� �˻��ϰ� ������ ��� ����
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
** ���� ���� ������ �׻� �������� �ʵ���
** �� ��ǥ�� �ڼ��´�.
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
** ���������� ���� ���� X ��ǥ ����
** ----------------------------------------
*/

    public int getLastX() {
        return lastX;
    }

/*
** ----------------------------------------
** ���������� ���� ���� Y ��ǥ ����
** ----------------------------------------
*/

    public int getLastY() {
        return lastY;
    }
}
