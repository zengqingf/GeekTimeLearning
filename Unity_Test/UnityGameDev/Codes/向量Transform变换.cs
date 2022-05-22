public class TestCode
{
    /*
    向量绕水平轴旋转
    Quaternion.AngleAxis(angle,Vector3.up) * player.forward

    Unity内置方法： Quaternion.AngleAxis(float angle, Vector3 axis)
    p1： 旋转角度，角度大于0，顺时针旋转；角度小于0，逆时针旋转（以坐标原点旋转）
    p2: 旋转轴
    */
    public void PlayerJoystickRotate()
    {
        Vector2 v2 = (dragPos - beginPos).normalized;
        float len = Vector2(v2.x, v2.y);
        float angle = Mathf.Atan2(v2.x, v2.y) * Mathf.Rad2Deg;
        angle = angle < 0 ? 360 + angle : angle;
        Vector3 playerMoveDir =  Quaternion.AngleAxis(angle,Vector3.up) * player.forward;
        player.Move(playerMoveDir * Time.deltaTime * moveSpeed);
    }

    
}
