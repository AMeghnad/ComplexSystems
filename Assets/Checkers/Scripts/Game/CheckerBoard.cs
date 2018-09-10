
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerBoard : MonoBehaviour
{
    #region Singleton
    // "Instance" keyword variable can be accessed everywhere
    public static CheckerBoard Instance;
    private void Awake()
    {
        // Set 'this' class as the first instance
        Instance = this;
    }
    #endregion

    #region Variables
    [Header("Game Logic")]
    public Piece[,] pieces = new Piece[8, 8]; // 2D Array - https://www.cs.cmu.edu/~mrmiller/15-110/Handouts/arrays2D.pdf
    public GameObject whitePiecePrefab, blackPiecePrefab; // Prefabs to spawn
    // Offset values of the board
    public Vector3 boardOffset = new Vector3(-4f, 0f, -4f);
    public Vector3 pieceOffset = new Vector3(.5f, .125f, .5f);
    public LayerMask hitLayers;
    public float rayDistance = 25f;

    private bool isWhite; // Is the current character white?
    private bool isWhiteTurn; // Is it white's turn?
    private bool hasKilled; // Has the player killed a piece?
    private Piece selectedPiece; // Current selected piece
    private List<Piece> forcedPieces; // List storing the pieces that are forced moves
    private Vector2 mouseOver; // Mouse over value
    private Vector2 startDrag; // Position of start drag
    private Vector2 endDrag; // Position of end drag
    #endregion

    #region Unity Events
    void Start()
    {
        // Generate the board on startup
        GenerateBoard();
    }

    void Update()
    {
        UpdateMouseOver();

        // Is is white's turn or black's turn?
        if (isWhite ? isWhiteTurn : !isWhiteTurn)
        {
            // Convert coordinates to int (again to be sure)
            int x = (int)mouseOver.x;
            int y = (int)mouseOver.y;

            //Select the piece - void SelectPiece(int x, int y)
            // If mousebutton down
            if (Input.GetMouseButtonDown(0))
            {
                // SelectPiece(x,y)
                SelectPiece(x, y);
            }

            // Is there a selectedPiece currently?
            if (selectedPiece != null)
            {
                // Update the drag position
                UpdatePieceDrag(selectedPiece);

                // If mouse up (mouse button released)
                if (Input.GetMouseButtonUp(0))
                {
                    endDrag = mouseOver;
                    // Move piece physically
                    TryMove((int)startDrag.x, (int)startDrag.y, (int)endDrag.x, (int)endDrag.y);
                }
            }
        }
    }
    #endregion

    #region Generators
    void GeneratePiece(bool isWhite, int x, int y)
    {
        GameObject prefab = isWhite ? whitePiecePrefab : blackPiecePrefab; // Which prefab is the piece?
        GameObject clone = Instantiate(prefab) as GameObject; // Instantiate the prefab 
        clone.transform.SetParent(transform); // Make checkerboard the parent of new piece
        Piece pieceScript = clone.GetComponent<Piece>(); // Get the "Piece" Component from clone ('Piece' needs to be attached to prefabs)
        pieces[x, y] = pieceScript; // Add piece component to array
        MovePiece(pieceScript, x, y); // Move the piece to correct world position
    }
    // Generate the board pieces
    void GenerateBoard()
    {
        // Generate white team
        for (int y = 0; y < 3; y++)
        {
            // If the remainder of /2 is zero, it is true
            // % = modulo - https://www.dotnetperls.com/modulo
            bool oddRow = (y % 2 == 0);
            // Loop through 8 and skip 2 every time
            for (int x = 0; // Initializer
                 x < 8;  // Condition
                 x += 2) // Incrementer / Iteration
                         // For Loops - https://www.tutorialspoint.com/csharp/csharp_for_loop.htm
            {
                // Generate piece here
                int desiredX = oddRow ? x : x + 1;
                int desiredY = y;
                GeneratePiece(true, desiredX, desiredY);
            }
        }

        // Generate black team
        for (int y = 7; y > 4; y--) // Go backwards from 7
        {
            bool oddRow = (y % 2 == 0); // Don't really need the extra '()' parenthesis
            for (int x = 0; x < 8; x += 2)
            {
                // Generate our piece
                int desiredX = oddRow ? x : x + 1;
                int desiredY = y;
                GeneratePiece(false, desiredX, desiredY);
            }
        }
    }
    #endregion

    #region Modifiers
    void SelectPiece(int x, int y)
    {
        // Check if x and y is outside of bounds of pieces array
        if (x < 0 || x >= 8 || y < 0 || y >= 8)
        {
            // return - exit function
            return;
        }

        // SET Piece p to pieces[x,y]
        Piece p = pieces[x, y];
        // If p exists and it is p's turn
        if (p != null && p.isWhite == isWhite)
        {
            // selectedPiece = p
            selectedPiece = p;
            // startDrag = mouseOver
            startDrag = mouseOver;
        }
    }

    // x1 = start x
    // x2 = end x
    // y1 = start y
    // y2 = end y

    void TryMove(int x1, int y1, int x2, int y2)
    {
        // Are any indexes out of bounds?
        if (x1 < 0 || x1 >= pieces.GetLength(0) || 
            x2 < 0 || x2 >= pieces.GetLength(0) || 
            y1 < 0 || y1 >= pieces.GetLength(1) || 
            y2 < 0 || y2 >= pieces.GetLength(1))
        {
            return;
        }

        // Is there a selectedPiece?
        if (selectedPiece != null)
        {
            // Move the piece
            MovePiece(selectedPiece, x2, y2);
            // Update array
            Piece temp = pieces[x1, y1]; // save original to temp
            pieces[x1, y1] = pieces[x2, y2]; // replace original with new
            pieces[x2, y2] = temp; // replace second with temp
            // Set selected to null
            selectedPiece = null;
        }
    }

    void MovePiece(Piece pieceToMove, int x, int y)
    {
        // Move the piece to world coordinates using x and y + offsets
        Vector3 coordinate = new Vector3(x, 0f, y);
        pieceToMove.transform.position = coordinate + boardOffset + pieceOffset;
    }
    #endregion

    #region Updaters
    void UpdateMouseOver()
    {
        // Does the main not camera exist?
        if (Camera.main == null)
        {
            Debug.Log("Unable to find Main Camera");
            // Exit the whole function
            return;
        }

        // Generate ray from mouse input to world
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Perform raycast
        if (Physics.Raycast(camRay, out hit, rayDistance, hitLayers))
        {
            // Convert world position to an array index (by converting to int aswell)
            mouseOver.x = (int)(hit.point.x - boardOffset.x);
            mouseOver.y = (int)(hit.point.z - boardOffset.z);
        }
        else
        {
            // '-1' means nothing was selected
            mouseOver.x = -1;
            mouseOver.y = -1;
        }
    }
    void UpdatePieceDrag(Piece pieceToDrag)
    {
        // Does the main camera not exist?
        if (Camera.main == null)
        {
            Debug.Log("Unable to find Main Camera");
            // Exit the functions
            return;
        }

        // Generate ray from mouse input to world
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Perform raycast
        if (Physics.Raycast(camRay, out hit, rayDistance, hitLayers))
        {
            // Start dragging the piece and move it just above the cursor
            pieceToDrag.transform.position = hit.point + Vector3.up;
        }
    }
    #endregion
}
