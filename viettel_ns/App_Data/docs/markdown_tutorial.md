
read sites for mastering markdown editor

https://guides.github.com/features/mastering-markdown/#examples

http://agea.github.io/tutorial.md/

https://help.github.com/articles/basic-writing-and-formatting-syntax/

Introduction
------------

Markdown is a markup language designed to be simple enough to let anyone write structured documents without the need of a visual editor

I **strongly** encourage you to change the source of the various parts to see what happens (the output will change as you type)



Basic styles
------------

With this markup you can obtain *simple emhpasis* (usually rendered in italic text), **strong emphasis** (usually rendered in bold text) or `source code` text (usually rendered in monospaced text)

You may use also _this_ or __this__ notation to emphatize text, and you can use all them _**`together`**_ (and you can mix `*` and `_` )

If you look at the source code you may note that
even 
if 
you 
break 
the 
lines,
the text is kept together
in a single paragraph

 Paragraphs are delimited by blank lines, leading and trailing spaces are removed 

You may force a line break with two spaces  
or with a `\`\
at the end of the line

Links
-----

- You can insert links in text like [this](/tutorial.md)

- You may add a [title](http://agea.github.io/tutorial.md "Markdown Tutorial") to your link (can you see the tooltip?)

- If your link contains spaces you have to write the [link](<http://example.com/a space>) between `<>`

- You can use spaces and markup inside the [link **text**](http://agea.github.io/tutorial.md)

- Long links may decrease source readability, so it's posible to define all links somewhere in the document (the end is a good place) and just reference the [link][tutorial.md], you may also collapse the reference if it matches the link text (example:  [tutorial.md][])

- You may also write directly the link: <http://agea.github.io/tutorial.md>

- It will work also for email addresses: <email@example.com> (you may write vaild email links also using [mailto](mailto:email@example.com) as protocol)


[tutorial.md]: http://agea.github.io/tutorial.md


Images
------

Syntax for images is like the syntax for links, but with a `!` before:

![alt text](img/1.png "image title")

![](img/2.png)

![ref]

[ref]: img/3.png

Lists
-----

To define a list of items, just put a `*`, a `-`, or a `+` at the start of the line of each item of the list followed by at least a space, to end the list, leave a blank line

* red
* green
* blue

- white
- grey
- black
+ yellow
+ pink
+ orange

You can also define numbered list, putting a number followed by a `.` or a `)` and a space at the start of the line (you may use any number, the first one is taken to start counting, then it will increment by one):
 
3.
2. you may leave blank items
1) or start
1) again

You can insert any block inside a list, you have to respect the indentation of the text of the list item

- A *paragraph* of text
  (spanning multiple lines),
  
  ```
  fenced code,
  ```
   
      indented code (4 spaces + 2 spaces for the list 
      indentation, one blank line above, one below),

  > quotes,
   
  - another 
    * list
      + (and so on...),
      
  - ### or headers
    
Headers
-------

There are two ways to define headers:

The biggest possible header
===========================

# You can also use this markup

(I prefer the first one as it's more readable when looking directly at the source code)
 
A sub heading
-------------
 
## This is the alternative format

### Then you can go smaller

#### And smaller

##### And even smaller

###### No, you can't go smaller than this

The good thing is that many tools that convert Markdown in HTML or PDF are able to generate the index of your document, or links to the headers automatically (like Github does on the [source](http://git.io/vfz98) of Markdown files)

lockquotes
-----------

> In this way you define a quoted block of text.
You can skip the initial `>` in intermediate lines 
if you are in the same paragraph
>
>> (you may nest levels)

    > but you can't indent with more than 3 spaces