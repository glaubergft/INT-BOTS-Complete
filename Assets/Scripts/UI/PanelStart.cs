using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UINavigation
{
    public class PanelStart : Panel
    {
        [SerializeField]
        StartManager startManager;

        [SerializeField]
        CharacterCustomizer customizer;

        [SerializeField]
        Text lblWarning;

        [SerializeField]
        ButtonHandler btnFindPlayers;

        [SerializeField]
        Dropdown ddlMatchFor;

        public InputField txtNickname;

        PanelStart()
        {
            Opened += PanelStart_Opened;
        }

        private void PanelStart_Opened()
        {
            lblWarning.enabled = false;
            btnFindPlayers.enabled = false;
            txtNickname.text = Player.NickName;
        }

        public void ChangeMyCharacter(GameObject menuButton)
        {
            Character selectedCharacter = menuButton.GetComponent<CharacterButton>().CharacterReference;
            Player.SelectedCharacter = selectedCharacter;
            customizer.UpdateCharacter(selectedCharacter);
        }

        public void btnFindPlayers_Click()
        {
            btnFindPlayers.GetComponent<ButtonHandler>().clickSound.Play();

            if (txtNickname.text.Equals(string.Empty))
            {
                StartCoroutine(showWarning());
                return;
            }

            RoomManager.MaxPlayers = byte.Parse(ddlMatchFor.options[ddlMatchFor.value].text);

            btnFindPlayers.enabled = true;
            btnFindPlayers.Submit();
            startManager.FindPlayers();
        }

        IEnumerator showWarning()
        {
            lblWarning.enabled = true;
            yield return new WaitForSeconds(2);
            lblWarning.enabled = false;
        }
    }
}