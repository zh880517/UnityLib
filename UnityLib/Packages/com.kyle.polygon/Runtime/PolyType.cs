namespace Polygon
{
    public enum PolyType
    {
        Area = 0,//有效区域,处于边缘内部的为有效区域
        Hollow = 1,//空洞,不可行走，但是没有碰撞
        Obstacle = 2,//障碍物。不可行走，有碰撞
    }
}
