namespace * Common

service ChatService
{
  string Say(1: string thing),

  list<string> GetList(
    1: string function2_arg1,
    2: i32 function2_arg2,
    3: list<string> function2_arg3
  ),
}