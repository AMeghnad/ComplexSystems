
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

    // Use this for initialization
    void Start()
    {
        // Generate the board on startup
        GenerateBoard();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMouseOver();

        // Is it white's turn or black's turn?
        if (isWhite ? isWhiteTurn : !isWhiteTurn)
        {
            // Convert coordinates to int (again to be sure)
            int x = (int)mouseOver.x;
            int y = (int)mouseOver.y;
            // Is there a selectedPiece currently?
            if (selectedPiece != null)
            {
                // Update the drag position
                UpdatePieceDrag(selectedPiece);
            }
        }
    }


    void MovePiece(Piece pieceToMove, int x, int y)
    {
        // Move the piece to world coordinates using x and y + offsets
        Vector3 coordinate = new Vector3(x, 0, y);
        pieceToMove.transform.position = coordinate + boardOffset + pieceOffset;
    }

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
            bool oddRow = (y % 2 == 0);
            // Loop through 8 and skip 2 every time
            for (int x = 0; x < 8; x += 2)
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

    void UpdateMouseOver()
    {
        if (Camera.main == null)
        {
            Debug.Log("Unable to find Main Camera");
            return;
        }

        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(camRay, out hit, rayDistance, hitLayers))
        {
            // Convert world position to array index
            mouseOver.x = (int)(hit.point.x - boardOffset.x);
            mouseOver.y = (int)(hit.point.z - boardOffset.z);
        }
    }

    void UpdatePieceDrag(Piece pieceToDrag)
    {
        if (Camera.main == null)
        {
            Debug.Log("Unable to find Main Camera");
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
}
