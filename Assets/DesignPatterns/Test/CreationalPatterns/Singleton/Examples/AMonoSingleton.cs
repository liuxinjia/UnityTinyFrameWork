using UnityEngine;

public class TestMonoSingleton<T> : SingletonBehaviour<T>where T : SingletonBehaviourBase
{
    int order = 0;
    public void Test() => Debug.Log(this.GetType());

    // private void Awake()
    // {
    //     BoostrapService.Cost += BoostrapService.Cost * this.Order * 1000;
    // }

    // // private void OnEnable() {

    // // }

    // private void Start()
    // {
    //     BoostrapService.Cost += BoostrapService.Cost * this.Order * 100;
    // }

    // private void Update()
    // {
    //     BoostrapService.Cost += BoostrapService.Cost * this.Order * 0.0001;
    // }

    // private void OnDestroy()
    // {
    //     BoostrapService.Cost += BoostrapService.Cost * this.Order * 10000;
    // }
}


public class AMonoSingleton : TestMonoSingleton<AMonoSingleton>
{


}

public class BMonoSingleton : TestMonoSingleton<BMonoSingleton>
{



}
public class CMonoSingleton : TestMonoSingleton<CMonoSingleton>
{



}
public class DMonoSingleton : TestMonoSingleton<DMonoSingleton>
{

    public void Ta() { }

}