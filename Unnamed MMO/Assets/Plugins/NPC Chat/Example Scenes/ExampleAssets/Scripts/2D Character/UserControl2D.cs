using UnityEngine;
using System.Collections;

[RequireComponent(typeof (CharacterController2D))]
public class UserControl2D : MonoBehaviour {
		
		private CharacterController2D m_Character;
		private bool m_Jump;

		private void Awake() {
			m_Character = GetComponent<CharacterController2D>();
		}

		private void Update() {
			if (!m_Jump) {
				// Read the jump input in Update so button presses aren't missed.
				m_Jump = Input.GetButtonDown("Jump");
			}
		}

		private void FixedUpdate() {
			// Read the inputs.
			bool crouch = Input.GetKey(KeyCode.LeftControl);
			float h = Input.GetAxis("Horizontal");
			// Pass all parameters to the character control script.
			m_Character.Move(h, crouch, m_Jump);
			m_Jump = false;
		}

}