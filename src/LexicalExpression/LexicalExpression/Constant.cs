namespace LexicalExpression
{
    /// <summary>
    ///     常量
    /// </summary>
	internal static class Constants
    {
        // 用于词法解释器的常量
        public const int Error = -1;       // 规则中错误
        public const int End = 0;       // 规则串结束
        public const int Hz = 1;       // 单词串
        public const int Eng = 2;       // 英文单词串
        public const int Number = 3;       // 数字串

        public const int Star = 4;     // '*' 
        public const int Percent = 5;     // '%' 
        public const int Slash = 6;     // '/' 
        public const int Equal = 7;     // '=' 
        public const int Plus = 8;     // '+' 
        public const int Or = 9;      // '|'
        public const int And = 10;      // '&'
        public const int Not = 11;      // '!'
        public const int Lparen = 12;       // '('
        public const int Rparen = 13;       // ')'
        public const int Lbracket = 14;     // '['
        public const int Rbracket = 15;    // ']'
        public const int Lbrace = 16;     // '{'
        public const int Rbrace = 17;    // '}'
        public const int Hashtag = 18;     // '#'
        public const int Lhash = 19;     // '#L'
        public const int Rhash = 20;     // '#R'
        public const int Colon = 21;     // '：'
        public const int Address = 22;   //'@'
        public const int Minus = 23;    //'-'
        public const int Space = 24;    //' '
        public const int Mark = 25;     //'^'
        public const int Dollar = 26;    //'$'
        public const int Comma = 27;    //','

        // 用于规则定义中的常量
        public const int BasicRuleItem = 1;     // 1：基本项
        public const int SkipRuleItem = 2;     // 2：越过项
        public const int OptionRuleItem = 3;     // 3：重复可选项
        public const int LeftHashRuleItem = 4;     // 4：左标记#L
        public const int RightHashRuleItem = 5;     // 5：右标记#R
        public const int StartPosRuleItem = 6;        //起始位置标记项
        public const int EndPosRuleItem = 7;        //结束位置标记项

        //WordItem类别
        public const int IsWordExpr = 1;
        public const int IsWordAtom = 0;
        //LabelAtom类别
        public const int IsLabelExpr = 1;
        public const int IsLabelStr = 0;

        //扫描方向
        public const bool Forward = true;
        public const bool Backward = false;
    }
}
