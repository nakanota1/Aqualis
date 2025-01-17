﻿(*
Copyright (c) 2022 Jun-ichiro Sugisaka

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*)
namespace Aqualis
    
    open System
    open System.IO
    open System.Text
    open Aqualis_base

    [<AutoOpen>]
    module print_ax =
        ///<summary>画面表示</summary>
        type print with
            
            ///<summary>1個の項目を画面表示</summary>
            static member c (s:ax1) = 
                iter.range _1 s.size1 <| fun i -> 
                    print.s[i;s.[i]]
                    
            ///<summary>1個の項目を画面表示</summary>
            static member c (s:ax2) = 
                iter.range _1 s.size1 <| fun i -> 
                    iter.range _1 s.size2 <| fun j -> 
                        print.s[i;j;s.[i,j]]
                        
            ///<summary>1個の項目を画面表示</summary>
            static member c (s:ax3) = 
                iter.range _1 s.size1 <| fun i -> 
                    iter.range _1 s.size2 <| fun j -> 
                        iter.range _1 s.size3 <| fun k -> 
                            print.s[i;j;k;s.[i,j,k]]
                            
            ///<summary>1個の項目を画面表示</summary>
            static member c (s:num1) = 
                iter.range _1 s.size1 <| fun i -> 
                    print.s[i;s.[i]]
                    
            ///<summary>1個の項目を画面表示</summary>
            static member c (s:num2) = 
                iter.range _1 s.size1 <| fun i -> 
                    iter.range _1 s.size2 <| fun j -> 
                        print.s[i;j;s.[i,j]]
                        
            ///<summary>1個の項目を画面表示</summary>
            static member c (s:num3) = 
                iter.range _1 s.size1 <| fun i -> 
                    iter.range _1 s.size2 <| fun j -> 
                        iter.range _1 s.size3 <| fun k -> 
                            print.s[i;j;k;s.[i,j,k]]
                            