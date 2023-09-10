namespace CustomMenu;

public class TableSettings
{
    public bool ColumnAlignLeft = true;
    public bool RowAlignLeft = true;
    
    public char VerticalDivider = '\u2503';
    public char HorizontalDivider = '\u2501';
    public char ColumnDivider = '\u2501';
    public char ColumnIntersection = '\u254b';
    public char TopDivider = '\u2501';
    public char BottomDivider = '\u2501';
    public char LeftDivider = '\u2503';
    public char RightDivider = '\u2503';
    public char IntersectionChar = '\u254b';
    public char TopLeftChar = '\u250f';
    public char TopRightChar = '\u2513';
    public char BottomLeftChar = '\u2517';
    public char BottomRightChar = '\u251b';
    public char TopIntersection = '\u2533';
    public char BottomIntersection = '\u253b';
    public char LeftIntersection = '\u2523';
    public char RightIntersection = '\u252b';
    
    public string EmptyCellFill = " ";
    public string autoGenerateIdColumnName = "#";
    public string InfoAfterTableDivider = "   ";
    
    public int ColumnTopMargin = 1;
    public int ColumnBottomMargin = 1;
    public int CellTopMargin = 0;
    public int CellBottomMargin = 0;
    public int CellLeftMargin = 1;
    public int CellRightMargin = 1;
    
    public bool DrawHorizontalDividers = true;
    public bool DrawVerticalDividers = true;
    public bool DrawColumnDivider = true;
    public bool DrawTopDivider = true;
    public bool DrawBottomDivider = true;
    public bool DrawLeftDivider = true;
    public bool DrawRightDivider = true;
    public bool EmptyCellFullFill = true;

    public ConsoleColor NameColor = ConsoleColor.DarkCyan;
    public ConsoleColor ColumnNameColor = ConsoleColor.Green;
    public ConsoleColor ItemColor = ConsoleColor.Magenta;
    public ConsoleColor DividerColor = ConsoleColor.DarkBlue;
    public ConsoleColor InfoAfterTableColor = ConsoleColor.DarkGray;
    public ConsoleColor EmptyCellFillColor = ConsoleColor.Gray;
};

public class SingleAnswerSettings
{
    public string Divider = ": ";
    public ConsoleColor NameColor = ConsoleColor.DarkCyan;
    public ConsoleColor AnswerColor = ConsoleColor.Magenta;
    public ConsoleColor DividerColor = ConsoleColor.DarkCyan;
}

public class Menu
{
    public delegate void MainLoopOptionDelegate(); // delegate for options in main loop
    
    private List<string> headerRows { get; set; } = new List<string>(); // header options
    private string headerDivider { get; set; } = ""; // divider between header options
    private string headerEndString { get; set; } = "\n"; // string in the end of the header
    private Dictionary<ConsoleKey, MainLoopOptionDelegate> mainLoopOpitons = new Dictionary<ConsoleKey, MainLoopOptionDelegate>();
    private bool mainLoopActive = false;
    
    // settings
    public TableSettings TableSettings = new TableSettings();
    public SingleAnswerSettings SingleAnswerSettings = new SingleAnswerSettings();

    public Menu()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
    }
    
    public void addHeaderRow(List<string> rows)
    {
        foreach (string row in rows)
        {
            addHeaderRow(row);
        }
    }
    
    public void addHeaderRow(string row)
    {
        headerRows.Add(row);
    }

    public void setHeaderDivider(string divider)
    {
        headerDivider = divider;
    }

    public void setHeaderEndString(string endString)
    {
        headerEndString = endString;
    }

    public void printHeader()
    {
        for (int i = 0; i < headerRows.Count; i++)
        {
            Console.Write(headerRows[i]);
            if (i != headerRows.Count - 1)
            {
                Console.Write(headerDivider);
            }
        }
        Console.Write(headerEndString);
    }

    public void addMainLoopOption(Dictionary<ConsoleKey, MainLoopOptionDelegate> options)
    {
        foreach (KeyValuePair<ConsoleKey,MainLoopOptionDelegate> option in options)
        {
            mainLoopOpitons[option.Key] = option.Value;
        }
    }

    private int getColumnWidth(List<string> columns, List<List<string>> rows, int index)
    {
        int max = columns[index].Length;
        foreach (List<string> row in rows) if (row[index].Length > max) max = row[index].Length;
        return max;
    }

    private void drawHorizontalDivider(List<string> columns, List<List<string>> rows, char leftChar, char rightChar, char divider, char intersection)
    {
        if (TableSettings.DrawLeftDivider) print(leftChar, TableSettings.DividerColor);
        for (int i = 0; i < columns.Count; i++)
        {
            for (int j = 0; j < getColumnWidth(columns, rows, i) + TableSettings.CellLeftMargin + TableSettings.CellRightMargin; j++) 
                print(divider, TableSettings.DividerColor);
            if (i != columns.Count - 1)
            {
                if (TableSettings.DrawVerticalDividers) print(intersection, TableSettings.DividerColor);
                else print(divider, TableSettings.DividerColor);
            }
        }
        if (TableSettings.DrawRightDivider) print(rightChar, TableSettings.DividerColor);
    }

    private void drawHorizontalMargin(List<string> columns, List<List<string>> rows)
    {
        if (TableSettings.DrawLeftDivider) print(TableSettings.LeftDivider, TableSettings.DividerColor);
        for (int i = 0; i < columns.Count; i++)
        {
            for (int j = 0; j < getColumnWidth(columns, rows, i) + TableSettings.CellLeftMargin + TableSettings.CellRightMargin; j++) 
                Console.Write(" ");
            if (i != columns.Count - 1) print(TableSettings.VerticalDivider, TableSettings.DividerColor);
        }
        if (TableSettings.DrawRightDivider) print(TableSettings.RightDivider, TableSettings.DividerColor);
        Console.WriteLine();
    }

    private void print(string str, ConsoleColor foregroundColor=ConsoleColor.White)
    {
        Console.ForegroundColor = foregroundColor;
        Console.Write(str);
        Console.ResetColor();
    }
    
    private void print(char str, ConsoleColor foregroundColor=ConsoleColor.White)
    {
        Console.ForegroundColor = foregroundColor;
        Console.Write(str);
        Console.ResetColor();
    }
    
    private void printLine(string str, ConsoleColor foregroundColor=ConsoleColor.White)
    {
        Console.ForegroundColor = foregroundColor;
        Console.WriteLine(str);
        Console.ResetColor();
    }
    
    private void printLine(char str, ConsoleColor foregroundColor=ConsoleColor.White)
    {
        Console.ForegroundColor = foregroundColor;
        Console.WriteLine(str);
        Console.ResetColor();
    }

    public void printSingleAnswer(string name, string answer)
    {
        print(name, SingleAnswerSettings.NameColor);
        print(SingleAnswerSettings.Divider, SingleAnswerSettings.DividerColor);
        printLine(answer, SingleAnswerSettings.AnswerColor);
    }

    private void drawInfoAfterTable(bool showRowNumber, bool showColumnNumber, int rows, int columns, List<string> additionalInfo)
    {
        if (showRowNumber)
        {
            print($"Rows: {rows}", TableSettings.InfoAfterTableColor);
            Console.Write(TableSettings.InfoAfterTableDivider);
        }
        if (showColumnNumber)
        {
            print($"Columns: {columns}", TableSettings.InfoAfterTableColor);
            Console.Write(TableSettings.InfoAfterTableDivider);
        }
        if (additionalInfo != null)
        {
            foreach (string info in additionalInfo)
            {
                print(info, TableSettings.InfoAfterTableColor);
                Console.Write(TableSettings.InfoAfterTableDivider);
            }
        }
    }

    private void fillEmptyCell(List<string> columns, List<List<string>> rows, int index)
    {
        if (TableSettings.EmptyCellFill == "") TableSettings.EmptyCellFill = " ";
        
        int cellWidth = TableSettings.CellLeftMargin + TableSettings.CellRightMargin + getColumnWidth(columns, rows, index);
        int usedSpace = 0;
        while (usedSpace + TableSettings.EmptyCellFill.Length <= cellWidth)
        {
            print(TableSettings.EmptyCellFill, TableSettings.EmptyCellFillColor);
            usedSpace += TableSettings.EmptyCellFill.Length;
        }
        print(TableSettings.EmptyCellFill.Substring(0, cellWidth - usedSpace), TableSettings.EmptyCellFillColor);
    }

    private List<List<string>> preprocessRows(List<List<string>> rows)
    {
        int maxLength = rows.Max(item => item.Count);
        foreach (List<string> row in rows)
        {
            if (row.Count < maxLength) for(int i = 0; i < maxLength - row.Count; i++) row.Add("");
        }
        return rows;
    }
    
    public void printTable(
        List<string> columns, 
        List<List<string>> rows, 
        string name="",
        bool showRowNumber=false,
        bool showColumnNumber=false,
        bool autoGenerateId=false,
        List<string> additionalInfo=null,
        List<int> rowsToShow=null
    )
    {
        preprocessRows(rows);
        
        // auto generate id
        if (autoGenerateId)
        {
            columns.Insert(0, TableSettings.autoGenerateIdColumnName);

            for (int i = 0; i < rows.Count; i++) rows[i].Insert(0, i.ToString());
        }
        
        // draw name
        printLine(name, TableSettings.NameColor);
        
        
        // draw top divider
        if (TableSettings.DrawTopDivider)
        {
            drawHorizontalDivider(columns, rows, TableSettings.TopLeftChar, TableSettings.TopRightChar, TableSettings.TopDivider, TableSettings.TopIntersection);
            Console.WriteLine();
        }


        // draw columns
        for (int m = 0; m < TableSettings.ColumnTopMargin; m++) drawHorizontalMargin(columns, rows); // margin top
        if (TableSettings.DrawLeftDivider) print(TableSettings.LeftDivider, TableSettings.DividerColor);
        for (int i = 0; i < columns.Count; i++)
        {
            Console.ForegroundColor = TableSettings.ColumnNameColor;
            for (int m = 0; m < TableSettings.CellLeftMargin; m++) Console.Write(" "); // margin left
            Console.Write($"{{0,{(TableSettings.ColumnAlignLeft ? '-' : "")}{getColumnWidth(columns, rows, i)}}}",  columns[i]);
            for (int m = 0; m < TableSettings.CellRightMargin; m++) Console.Write(" "); // margin right
            Console.ResetColor();
            
            if (i != columns.Count - 1)
            {
                if (TableSettings.DrawVerticalDividers) print(TableSettings.VerticalDivider, TableSettings.DividerColor);
                else print(" ", TableSettings.DividerColor);
            }
        }
        if (TableSettings.DrawRightDivider) print(TableSettings.RightDivider, TableSettings.DividerColor);
        Console.WriteLine();
        for (int m = 0; m < TableSettings.ColumnBottomMargin; m++) drawHorizontalMargin(columns, rows); // margin bottom
        
        if (TableSettings.DrawColumnDivider)
        {
            drawHorizontalDivider(columns, rows, TableSettings.LeftIntersection, TableSettings.RightIntersection, TableSettings.ColumnDivider, TableSettings.ColumnIntersection);
            Console.WriteLine();
        }
        
        
        // draw rows
        int rowsPrinted = 0;
        for (int i = 0; i < rows.Count; i++)
        {
            if (rowsToShow == null || rowsToShow.Contains(i))
            {
                for (int m = 0; m < TableSettings.CellTopMargin; m++) drawHorizontalMargin(columns, rows); // margin top
                if (TableSettings.DrawLeftDivider) print(TableSettings.LeftDivider, TableSettings.DividerColor);
                for (int j = 0; j < rows[i].Count; j++)
                {
                    if (rows[i][j] != "")
                    {
                        Console.ForegroundColor = TableSettings.ItemColor;
                        for (int m = 0; m < TableSettings.CellLeftMargin; m++) Console.Write(" "); // margin left
                        Console.Write(
                            $"{{0,{(TableSettings.RowAlignLeft ? '-' : "")}{getColumnWidth(columns, rows, j)}}}",
                            rows[i][j]);
                        for (int m = 0; m < TableSettings.CellRightMargin; m++) Console.Write(" "); // margin right
                        Console.ResetColor();
                    }
                    else
                    {
                        fillEmptyCell(columns, rows, j);
                    }

                    if (j != columns.Count - 1)
                    {
                        if (TableSettings.DrawVerticalDividers)
                            print(TableSettings.VerticalDivider, TableSettings.DividerColor);
                        else print(" ", TableSettings.DividerColor);
                    }
                }

                if (TableSettings.DrawRightDivider) print(TableSettings.RightDivider, TableSettings.DividerColor);
                Console.WriteLine();
                for (int m = 0; m < TableSettings.CellBottomMargin; m++)
                    drawHorizontalMargin(columns, rows); // margin bottom
                
                rowsPrinted += 1;

                if (TableSettings.DrawHorizontalDividers && i != rows.Count - 1 && (rowsToShow == null || rowsPrinted < rowsToShow.Count))
                {
                    drawHorizontalDivider(columns, rows, TableSettings.LeftIntersection,
                        TableSettings.RightIntersection, TableSettings.HorizontalDivider,
                        TableSettings.IntersectionChar);
                    Console.WriteLine();
                }
            }
        }
        
        
        // draw bottom divider
        if (TableSettings.DrawBottomDivider)
        {
            drawHorizontalDivider(columns, rows, TableSettings.BottomLeftChar, TableSettings.BottomRightChar, TableSettings.BottomDivider, TableSettings.BottomIntersection);
            Console.WriteLine();
        }
        
        // additional info after table
        drawInfoAfterTable(
            showRowNumber, 
            showColumnNumber, 
            rowsPrinted, 
            autoGenerateId ? columns.Count - 1 : columns.Count,
            additionalInfo
            );
    }

    public void printTableColumn(
        List<string> columnNames,
        List<List<string>> columnValues,
        string name="",
        bool showRowNumber=false,
        bool showColumnNumber=false,
        bool autoGenerateId=false,
        List<string> additionalInfo=null,
        List<int> rowsToShow=null
        )
    {
        List<List<string>> rows = new List<List<string>>();
        for (int i = 0; i < columnValues.Max(item => item.Count); i++)
        {
            List<string> row = new List<string>();
            foreach (List<string> columnValue in columnValues)
            {
                if (columnValue.Count <= i) row.Add("");
                else row.Add(columnValue[i]);
            }
            rows.Add(row);
        }
        
        printTable(
            columnNames, 
            rows, 
            name, 
            showRowNumber:showRowNumber, 
            showColumnNumber:showColumnNumber,
            autoGenerateId:autoGenerateId,
            additionalInfo: additionalInfo,
            rowsToShow:rowsToShow
            );
    }

    public void printTableColumn(
        Dictionary<string, List<string>> columns,
        string name="",
        bool showRowNumber=false,
        bool showColumnNumber=false,
        bool autoGenerateId=false,
        List<string> additionalInfo=null,
        List<int> rowsToShow=null
        )
    {
        printTableColumn(
            columns.Keys.ToList(), 
            columns.Values.ToList(), 
            name, 
            showRowNumber, 
            showColumnNumber, 
            autoGenerateId,
            additionalInfo,
            rowsToShow
            );
    }

    public void startMainLoop(ConsoleKey quitKey=ConsoleKey.Q)
    {
        mainLoopActive = true;
        ConsoleKey key = ConsoleKey.NoName;
        while (key != quitKey && mainLoopActive)
        {
            printHeader();

            if (mainLoopOpitons.ContainsKey(key)) mainLoopOpitons[key]();
            
            key = Console.ReadKey().Key;
            Console.Clear();
        }
    }

    public void stopMainLoop()
    {
        mainLoopActive = false;
    }
}