

# This is a sample Python script.

# Press Umschalt+F10 to execute it or replace it with your code.
# Press Double Shift to search everywhere for classes, files, tool windows, actions, and settings.
# pip install spacy==2.3.5
import spacy
#import sys
#sys.path.append("../../Managed/")
import UnityEngine
# pip install spacy_sentiws
from spacy_sentiws import spaCySentiWS

def print_hi(name):
    # Use a breakpoint in the code line below to debug your script.
    print(f'Hi, {name}')  # Press Strg+F8 to toggle the breakpoint.


# Press the green button in the gutter to run the script.
if __name__ == '__main__':
    print("Start Sentiment Analysis")
    all_objects = UnityEngine.Object.FindObjectsOfType(UnityEngine.GameObject)
    for go in all_objects:
        if go.name[-1] != '_':
            go.name = go.name + '_'

    nlp = spacy.load("de")
    sentiws = spaCySentiWS(sentiws_path='data/sentiws/')
    nlp.add_pipe(sentiws)
    doc = nlp('Die Dummheit der Unterwerfung blüht in hübschen Farben.')

    for token in doc:
        print('{}, {}, {}'.format(token.text, token._.sentiws, token.pos_))

# See PyCharm help at https://www.jetbrains.com/help/pycharm/
