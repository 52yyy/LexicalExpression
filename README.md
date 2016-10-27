# LexicalExpression

---

## 简介 ( Introduction )

> 提供对产品和文档本身的总体的、扼要的说明

**特点**

*	测试字符串内的模式（规则）并输出
例如：可以测试输入字符串，以查看字符串内是否出现电话号码模式或信用卡号码模式。这称为数据验证。
*	抽取字符串中的子串。
可以查找文档内或输入域内特定的文本。
*	抽取特定关系的对象。
例如：抽取句子中有并列关系，有比较关系或者有修饰关系的词语。


## 快速上手 ( Getting Started )

> 如何快速地使用产品

## 入门篇 ( Basics )

> 又称“使用篇”，提供初级的使用教程

``` c#
/* 示例 */
public void Demo()
{
  LexSentence sen;
  Lexex ruleParser = new Lexex("Your Rule Path");
  Match causeCollection = ruleParser.Run(sen, 0);
  while (causeCollection.Success)
  {
    LexSentence[] causes = causeCollection.MatchCollection["Right Label"].ToArray();
    causeCollection = causeCollection.GetNext();
  }
}

```

### 环境准备 ( Prerequisite )

> 软件使用需要满足的前置条件

### 安装 ( Installation )

> 软件的安装方法

### 设置 ( Configuration )

> 软件的设置

## 进阶篇 ( Advanced )

> 又称“开发篇”，提供中高级的开发教程

### 语法

词法表达式中的规则是由等式来描述的，其中，等号“=”作为标识等式的符号用于唯一区分一条规则，单条规则的语法如下：

```
<匹配规则> = <输出规则>
```

在一条词法表达式语句中，位于等式左边的部分被称为“匹配规则”，“匹配规则”用于匹配字符串，只有符合匹配条件的字符串才能够被输出。相对地，位于等式右边的部分被称为“输出规则”，当词法表达式匹配到符合条件的字符串时，“输出规则”用于决定匹配结果所输出的内容以及格式。

### 匹配规则

匹配规则位于整条规则等式的左边，故又称作“规则左部”。通常，匹配规则由若干“规则项”组成，并且，匹配规则通过加号连接符“+”将各个规则项前后串连起来，有且仅有整条被串连起来的规则项与待匹配语句中的词语依次相邻并且全部匹配，才认为整条规则匹配成功。匹配规则的语法如下：

```
<规则项1> + <规则项2> + <规则项3> + ……
```

所谓规则项，是匹配规则部分的基本单位，规则项之间无关系，每一个规则项独立地与待匹配语句的词语匹配，并根据成功匹配与否返回相应的结果。规则项根据独立匹配词语个数或方式的不同，又分为`基本项`和`越过项`两种。
词法表达式是一种类似于正则表达式的一种文本模式，包括基本项和越过项。其中越过项又可以由若干个基本项组成。基本项和越过项分别由普通字符和特殊字符构成。通过使用基本项和越过项的任意组合，描述在识别或者抽取文本时要匹配的一个或者多个字符串。

下表包含了基本项和越过项的完整列表以及它们在规则表达式上下文中的作用：
|表达式|意义说明|
|---|---|
|\*/%|一个基本项，由'/'分隔的两部分组成：'\*'表示文本，'%'表示标签|
|A = B|分隔表达式的左部A和右部B，A表示要匹配的规则，B则是要输出的规则由大括号构成|
|A + B|AB为任意的项，该表达式是按顺序连接两项，两者以顺序关系匹配时满足规则要求|
|x \| y|or 表达式，表示x或y匹配到任意一个则满足规则。例如，'z|food' 匹配“z”或“food”|
|(\*)|标记子表达式的开始和结束。表示括号中的构成一个子表达式|
|#[A]|越过项，匹配括号中的表达式A一次或者多次，其中A为一个基本项|
|m:n|标记限定符，限定匹配项出现的次数。表示最低m次，最高n次|
|!(A)|not 表达式，表示不符合表达式A的句子符合该项的规则。A可以是字符或者表达式|
|A & B|and 表达式，表示ＡＢ表示的规则同时满足才满足该表达式的规则|
|{A : B}|表达式右部，ＡＢ为输出字典结构的键值（key：value）对。AB可以是一个子表达式
其中A可以是特定字符，B可以使用左部各项的index|
|{ }|输出结果表达式边界限定,表示该输出表达式为字典数据结构|

**基本项**

基本项是匹配规则的最小单位，一个基本项用于匹配一个词语。词法表达式能够对词语以及词语的标签建立匹配规则的原因就是基于基本项。一个基本项由一个词表达式句柄和一个标记表达式句柄组成，并使用反斜杠符号“/”连接两句柄。例如：

```
<词表达式句柄> / <标记表达式句柄>
```

若要匹配一个字符与标签的组合是否满足规则，则要制定一个基本项进行匹配。如要在下句 “我/t是/s中国人/n”，基本项如下：
```
我/%&^
```

会输出所有句子中满足“我”位于句首的情况并且不考虑标签。

**越过项**

越过项用于匹配符合要求的任意数量连续词语。由于需要匹配整条匹配规则才能算作成功，当遇到需要匹配的内容不相邻的情形，需要匹配规则中的匹配项能够越过若干无关的词语进行继续匹配，这时越过项就能够派上用场。
越过项由一个基本项以及固定格式的越过项标识符组成，一个标准的越过项语句格式如下：

```
#<下限>:<上限>[<基本项>]
```

其中，下限和上限分别表示可以越过的与基本项相匹配的词语数边界。此外，下限和上限数可以省略，被省略的情况下，下限将以0作为默认值，上限将以65000作为默认值。省略的越过项语句格式如下：

```
#<下限>[<基本项>]
#[<基本项>]
```

如要创建一个表达式，使其满足其中的子表达式都可以出现一次或者多次，请在方括号（[ 和 ]）内放置一个或多个基本项构成的子表达式，并在括号前加上“#”。基本项子表达式组合括在中括号内时，该列表称为“越过项表达式”。

```
#1:5[*/t] = {“越过项：”: 1}
```

左部为越过项，该越过项表示，最少出现1次，最多出现5次，且tag标签为‘t’的字符,并且输出整个符合该结构的字符串作为输出结果。

下表包含了特殊字符的完整列表以及他们在规则表达式中的作用。

**特殊标记符**

基本项的各个句柄中语句的内容是正式匹配词语的内容，分为普通标记符和特殊标记符。其中，普通标记符由数字、字母或中文汉字组成，在匹配是被当作为字符串进行直接匹配；特殊标记符则由一些符号表示，这些符号并不表示其本身含义，而是提供了额外特殊的标记含义，并且，在实战中需要尽量避免使用特殊标记符的原来意思，以免产生未知错误。具体地，特殊标记符与其含义对照表如下：

|字符|意义说明|
|---|---|
|\*|词语通配符，位于'/'之前的词表达式句柄，表示可以是任何词语|
|%|标记通配符，位于'/'之后的标记表达式句柄，表示可以是任何标记|
|\||或，表示'or'的关系|
|!|非，表示'not'的关系|
|&|和，表示'and'的关系，经常用于连接'^'和'$'|
|()|优先级标记符，跟算术规则类似|
|^|句首标记符|
|$|句尾标记符|

### 输出规则

规则解析是从左到右进行计算，并遵循优先级顺序，与算术表达式类似，符合人的思维，有助于理解，使用更方便。
下表从高都低的介绍规则表达式中各种运算符的优先级顺序。

|运算符|	意义说明|
|---|---|
|#[]|	越过项标示符|
|()	|括号|
|{}|	限定符|
|!|	非|
|&|	并|
|\||	或|
|+|	项连接符|

字符和标签具有高于替换运算符的优先级，使得“m|food”匹配“m”或“food”。若要匹配“mood”或“food”，请使用括号创建子表达式，从而产生“(m|f)ood”。

### 示例

## API ( Reference )

> 软件API的逐一介绍

## FAQ 

## 附录 ( Appendix )

### 名词解释 ( Glossary )

### 最佳实践 ( Recipes )

### 故障处理 ( Troubleshooting )

### 版本说明 ( ChangeLog )

### 反馈方式 ( Feedback )





