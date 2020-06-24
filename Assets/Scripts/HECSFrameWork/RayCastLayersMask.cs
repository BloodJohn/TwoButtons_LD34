using UnityEngine;

public class RayCastLayersMask
{
    public readonly LayerMask AllMasks;
    public readonly int DefaultLayer = LayerMask.NameToLayer("Default");
    public readonly int CharactersLayer = LayerMask.NameToLayer("Characters");
    public readonly int InteractiveComponentsLayer = LayerMask.NameToLayer("InteractiveComponents");

    private readonly LayerMask Default;
    private readonly LayerMask Characters;
    private readonly LayerMask InteractiveComponents;
    
    public RayCastLayersMask()
    {
        Default = 1 << DefaultLayer;
        Characters = 1 << CharactersLayer;
        InteractiveComponents = 1 << InteractiveComponentsLayer;

        AllMasks = Default | Characters | InteractiveComponents;
    }
}