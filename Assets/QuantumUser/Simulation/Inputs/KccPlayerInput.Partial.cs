namespace Quantum 
{
  public unsafe partial struct KccPlayerInput 
  {
    public static implicit operator Input(KccPlayerInput playerInput) 
    {
        Input input = default;
        
        input._a = playerInput.Jump;
        input.ThumbSticks.HighRes->_leftThumb = playerInput.MoveDirection;
        input.ThumbSticks.HighRes->_rightThumb.Pitch = playerInput.Pitch;
        input.ThumbSticks.HighRes->_rightThumb.Yaw = playerInput.Yaw;
        
        return input;
    }

    public static implicit operator KccPlayerInput(Input input) 
    {
        KccPlayerInput playerInput = default;
        
        playerInput.Jump = input._a;
        playerInput.MoveDirection = input.ThumbSticks.HighRes->_leftThumb;
        playerInput.Yaw = input.ThumbSticks.HighRes->_rightThumb.Yaw;
        playerInput.Pitch = input.ThumbSticks.HighRes->_rightThumb.Pitch;
        
        return playerInput;
    }
  }
}
