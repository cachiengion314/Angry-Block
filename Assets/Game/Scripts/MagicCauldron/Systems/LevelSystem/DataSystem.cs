using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
  [SerializeField] RandomDataObj[] dataObjs;
  public void SaveMagicCauldronData()
  {
    for (int i = 0; i < dataObjs.Length; ++i)
    {
      SaveData(_initOrders[i], _destinationOrders[i], i);
    }
  }
  public void SaveData(int[] InitOrders, int[] DestinationOrders, int level)
  {
    if (level > dataObjs.Length - 1) return;

    var datas = HoangNam.SaveSystem.LoadWith<RandomDatas>(KeyString.NAME_RANDOM_DATA_MAGICCAULDRON);
    if (datas.randomDatas == null)
    {
      datas = new RandomDatas();
      RandomData[] randomDatas = new RandomData[dataObjs.Length];
      datas.randomDatas = randomDatas;
    }

    int[] _currentEggHolder = new int[InitOrders.Length];
    for (int i = 0; i < _currentEggHolder.Length; i++) _currentEggHolder[i] = -1;

    datas.randomDatas[level].InitOrders = InitOrders;
    datas.randomDatas[level].CurrentEggHolder = _currentEggHolder;
    datas.randomDatas[level].DestinationOrders = DestinationOrders;

    HoangNam.SaveSystem.SaveWith(datas, KeyString.NAME_RANDOM_DATA_MAGICCAULDRON);
  }

  public void SaveEggData(int level, int PlacedIndex, int value)
  {
    if (level > dataObjs.Length - 1) return;

    var datas = HoangNam.SaveSystem.LoadWith<RandomDatas>(KeyString.NAME_RANDOM_DATA_MAGICCAULDRON);
    datas.randomDatas[level].CurrentEggHolder[PlacedIndex] = value;
    HoangNam.SaveSystem.SaveWith(datas, KeyString.NAME_RANDOM_DATA_MAGICCAULDRON);
  }

  public void SaveEggData(int level, int[] currentEggHolder)
  {
    if (level > dataObjs.Length - 1) return;

    var datas = HoangNam.SaveSystem.LoadWith<RandomDatas>(KeyString.NAME_RANDOM_DATA_MAGICCAULDRON);
    datas.randomDatas[level].CurrentEggHolder = currentEggHolder;
    HoangNam.SaveSystem.SaveWith(datas, KeyString.NAME_RANDOM_DATA_MAGICCAULDRON);
  }

  public void InitData()
  {
    var datas = HoangNam.SaveSystem.LoadWith<RandomDatas>(KeyString.NAME_RANDOM_DATA_MAGICCAULDRON);
    for (int i = 0; i < dataObjs.Length; i++)
    {
      dataObjs[i].InitOrders = datas.randomDatas[i].InitOrders;
      dataObjs[i].CurrentEggHolder = datas.randomDatas[i].CurrentEggHolder;
      dataObjs[i].DestinationOrders = datas.randomDatas[i].DestinationOrders;
    }
  }

  public RandomDataObj GetDataAt(int level)
  {
    return dataObjs[level];
  }
}