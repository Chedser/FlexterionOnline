
public class DisableEffect : Disablable
{

    private void OnEnable()
    {
        Invoke(nameof(Disable), timeToDisable);
    }

    public override void Disable()
    {

        this.gameObject.SetActive(false);

    }

}
