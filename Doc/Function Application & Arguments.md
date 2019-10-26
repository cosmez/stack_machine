Implementing a function call and argument passing can change in a Stack VM if the size of any argument is bigger than a WORD[^1] and if there is no static type information to infer the  arguments location.

.NET IL defines the shape of the arguments in the signature of the function.
```IL
.method int32 Sum (
    valuetype User/Value res,
    int32 x,
    int32 y
) cil managed 
```

If the user defined `struct` at location [0] has two int32's, that means `int32 x` its at location [2], and `int32 y` its at location [3]. 



[^1] Explain what is a WORD in this case.