mutable a = "global a";
mutable b = "global b";
mutable c = "global c";
{
  mutable a = "outer a";
  mutable b = "outer b";
  {
    mutable a = "inner a";
    print a;
    print b;
    print c;
  }
  print a;
  print b;
  print c;
}
print a;
print b;
print c;