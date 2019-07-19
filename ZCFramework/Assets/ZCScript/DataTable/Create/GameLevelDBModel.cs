using ZCFrame;

/// <summary>
/// GameLevel数据管理
/// </summary>
public partial class GameLevelDBModel : DataTableDBModelBase<GameLevelDBModel, GameLevelEntity>
{
    /// <summary>
    /// 文件名称
    /// </summary>
    public override string DataTableName { get { return "GameLevel"; } }

    /// <summary>
    /// 加载列表
    /// </summary>
    protected override void LoadList(MMO_MemoryStream ms)
    {
        //行数
        int rows = ms.ReadInt();
        //列数
        int columns = ms.ReadInt();

        for (int i = 0; i < rows; i++)
        {
            
            GameLevelEntity entity = new GameLevelEntity();
            entity.Id = ms.ReadInt();
            entity.ChapterID = ms.ReadInt();
            entity.Name = ms.ReadUTF8String();
            entity.SceneName = ms.ReadUTF8String();
            entity.SmallMapImg = ms.ReadUTF8String();
            entity.isBoss = ms.ReadInt();
            entity.Ico = ms.ReadUTF8String();
            entity.PosInMap = ms.ReadUTF8String();
            entity.DlgPic = ms.ReadUTF8String();
            entity.CameraRotation = ms.ReadUTF8String();
            entity.Audio_BG = ms.ReadUTF8String();

            m_List.Add(entity);
            if (m_Dic.ContainsKey(entity.Id))
            {
                UnityEngine.Debug.Log("包含  " + entity.Id);
            }
            else
            {
                m_Dic.Add(entity.Id, entity);

            }
           
            //m_Dic[entity.Id] = entity;
        }


    }
}