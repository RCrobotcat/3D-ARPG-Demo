using AIActor_RC;
using CleverCrow.Fluid.BTs.Trees;
using CleverCrow.Fluid.BTs.Tasks;

public class TestAIBehaviorChild : AIActor
{
    protected override void Start()
    {
        base.Start();

        InitAI();
    }

    protected override void Update()
    {
        if (brain != null)
        {
            brain.Tick();
        }
    }

    void InitAI()
    {
        // Create behavior tree
        brain = new BehaviorTreeBuilder(gameObject)
            .Do("Test Behavior Tree", () =>
            {
                return TaskStatus.Success; // Return success status
            }).Build();
    }
}