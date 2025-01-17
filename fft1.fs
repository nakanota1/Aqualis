﻿(*
Copyright (c) 2022 Jun-ichiro Sugisaka

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*)
namespace Aqualis
    
    open Aqualis_base
    
    module fft1 = 
        
        type fftw_plan1(name) =
            static member sname = "fftw_plan"
            new(name,c) =
                str.reg(fftw_plan1.sname,name,c)
                fftw_plan1(name)
            member __.name = name
            
        let fftshift_odd(a:ax1) =
            let n1 = a.size1
            let n2 = a.size1./2
            ch.z <| fun tmp ->
                tmp <== a[n2+1]
                iter.num n2 <| fun i ->
                    a[i+n2] <== a[i]
                    a[i] <== a[i+n2+1]
                a[a.size1] <== tmp
        
        let fftshift_even(a:ax1) =
            let n2 = a.size1./2
            ch.z <| fun tmp ->
                iter.num n2 <| fun i ->
                    tmp <== a[i+n2]
                    a[i+n2] <== a[i]
                    a[i] <== tmp
                    
        let ifftshift_odd(a:ax1) =
            let n1 = a.size1
            let n2 = n1./2
            ch.z <| fun tmp ->
                tmp <== a[n2+1]
                iter.num n2 <| fun i ->
                    a[n1-i+1-n2] <== a[n1-i+1]
                    a[n1-i+1] <== a[n1-i+1-n2-1]
                a[1] <== tmp
        
        let ifftshift_even(a:ax1) =
            let n2 = a.size1./2
            ch.z <| fun tmp ->
                iter.num n2 <| fun i ->
                    tmp <== a[i+n2]
                    a[i+n2] <== a[i]
                    a[i] <== tmp
                    
        let fftshift1(x:num1) =
            br.if2 (x.size1%2 .= 0)
                <| fun () ->
                    fftshift_even(ax1(x.size1,fun i -> x[i]))
                <| fun () ->
                    fftshift_odd(ax1(x.size1,fun i -> x[i]))
                        
        let ifftshift1(x:num1) =
            br.if2 (x.size1%2 .= 0)
                <| fun () ->
                    ifftshift_even(ax1(x.size1,fun i -> x[i]))
                <| fun () ->
                    ifftshift_odd(ax1(x.size1,fun i -> x[i]))
                        
        let private fft1(planname:string,data1:num1,data2:num1,fftdir:int) =
            p.param.option_("-lfftw3")
            p.param.option_("-I/usr/include")
            ch.ii <| fun (N,N2) -> 
                N<==data1.size1
                N2<==asm.toint(asm.floor(N/2.0))
                match p.param.lang with
                  |F ->
                    p.param.include_("'fftw3.f'")
                    let plan = var.i1(planname, 8)
                    if fftdir=1 then
                        p.param.codewrite("call dfftw_plan_dft_1d(" + plan.name + ", " + N.name + ", " + data1.name + ", " + data2.name + ", FFTW_FORWARD, FFTW_ESTIMATE )")
                        fftshift1(data1)
                        !"FFTを実行"
                        p.param.codewrite("call dfftw_execute(" + plan.name + ")")
                        fftshift1(data2)
                        p.param.codewrite("call dfftw_destroy_plan(" + plan.name + ")")
                    else
                        p.param.codewrite("call dfftw_plan_dft_1d(" + plan.name + ", " + N.name + ", " + data1.name + ", " + data2.name + ", FFTW_BACKWARD, FFTW_ESTIMATE )")
                        ifftshift1(data1)
                        !"FFTを実行"
                        p.param.codewrite("call dfftw_execute(" + plan.name + ")")
                        ifftshift1(data2)
                        p.param.codewrite("call dfftw_destroy_plan(" + plan.name + ")")
                  |C89|C99 ->
                    p.param.include_("\"fftw3.h\"")
                    let plan = fftw_plan1(planname)
                    if fftdir=1 then
                        p.param.codewrite(plan.name + " = fftw_plan_dft_1d(" + N.name + ", " + data1.name + ", " + data2.name + ", FFTW_FORWARD, FFTW_ESTIMATE);")
                        fftshift1(data1)
                        !"FFTを実行"
                        p.param.codewrite("dfftw_execute(" + plan.name + ")")
                        fftshift1(data2)
                        p.param.codewrite("dfftw_destroy_plan(" + plan.name + ")")
                    else
                        p.param.codewrite(plan.name + " = fftw_plan_dft_1d(" + N.name + ", " + data1.name + ", " + data2.name + ", FFTW_BACKWARD, FFTW_ESTIMATE);")
                        ifftshift1(data1)
                        !"FFTを実行"
                        p.param.codewrite("dfftw_execute(" + plan.name + ")")
                        ifftshift1(data2)
                        p.param.codewrite("dfftw_destroy_plan(" + plan.name + ")")
                  |T ->
                    p.param.codewrite(data2.name + " = \\mathcal{F}\left[" + data1.name + "\right]")
                  |H ->
                    p.param.codewrite(data2.name + " = <mi mathvariant=\"script\">F</mi><mfenced open=\"[\" close=\"]\">" + data1.name + "</mfenced>")
                  |NL ->
                    ()
                if fftdir=1 then
                    !"規格化"
                    iter.num N <| fun i ->
                        data2.[i]<==data2.[i]/N
                        
        let fft(planname:string,data1:num1,data2:num1) =
                fft1(planname,data1,data2,1)
                
        let ifft(planname:string,data1:num1,data2:num1) =
                fft1(planname,data1,data2,-1)
                