﻿(*
Copyright (c) 2022 Jun-ichiro Sugisaka

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*)
namespace Aqualis
    
    open System
    open System.IO
    open System.Text
    
    ///<summary>言語を指定</summary>
    type Language =
        ///<summary>Fortran</summary>
        |F
        ///<summary>C89</summary>
        |C89
        ///<summary>C99</summary>
        |C99
        ///<summary>LaTeX</summary>
        |T
        ///<summary>HTML</summary>
        |H
        ///<summary>未指定（コンパイル中でない）</summary>
        |NL
        
    ///<summary>変数の型を指定</summary>
    type Etype =
        ///<summary>整数型(バイト数)</summary>
        |It of int
        ///<summary>倍精度浮動小数点型</summary>
        |Dt
        ///<summary>複素数（倍精度）</summary>
        |Zt
        ///<summary>非数値</summary>
        |Nt
        ///<summary>構造体</summary>
        |Structure of string
        
    ///<summary>変数、配列とその次元の指定</summary>
    type VarType =
        ///<summary>変数</summary>
        |A0
        ///<summary>1次元配列(要素数)</summary>
        |A1 of int
        ///<summary>2次元配列(要素数1,要素数2)</summary>
        |A2 of int*int
        ///<summary>3次元配列(要素数1,要素数2,要素数3)</summary>
        |A3 of int*int*int
        
    ///<summary>条件分岐の種類を指定</summary>
    type Branch =
        ///<summary>if条件式</summary>
        |IF
        ///<summary>else if条件式</summary>
        |ELIF
        ///<summary>else条件式変数</summary>
        |ELSE
        
    ///<summary>設定のONまたはOFFを指定</summary>
    type Switch =
        |ON
        |OFF
        
    ///<summary>番号付き変数の管理</summary>
    type varlist () =
        let mutable counter_ = 0
        let mutable maxcounter_ = 0
        ///<summary>使用済み変数番号</summary>
        let mutable disposed_:string list = []
        ///<summary>最新の変数番号</summary>
        member __.counter with get() = counter_
        ///<summary>これまで生成された変数番号の最大値</summary>
        member __.maxcounter with get() = maxcounter_
        ///<summary>変数番号をインクリメント</summary>
        member private __.inc() = 
            counter_ <- counter_ + 1
            if counter_ > maxcounter_ then
                maxcounter_ <- counter_
            counter_
        ///<summary>変数番号をリセット</summary>
        member __.reset() =
            counter_ <- 0
        ///<summary>変数番号、使用済み変数番号をリセット</summary>
        member this.clear() =
            this.reset()
            disposed_ <- []
        ///<summary>使用可能な変数番号を取得</summary>
        member this.getvar(f:int->string) =
            match disposed_ with
              |[] ->
                //リストが空の場合は新規作成
                //変数名カウンタをインクリメントして変数生成
                f <| this.inc()
              |x::y ->
                //使用可能な変数がある場合はリストから取得（リストからは外す）
                disposed_ <- y
                x
        ///<summary>変数番号nを使用済みリストに入れる</summary>
        member __.dispose(n:string) =
            disposed_ <- n::disposed_
            