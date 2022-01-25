import spacy
from spacy.lang.de import German

from WordOccurrence import WordOccurrence


class NLP:
    def __init__(self):
        self.nlp = German()
        self.nlp.add_pipe(self.nlp.create_pipe('sentencizer'))

    def get_nlp_doc(self, text):
        return self.nlp(text)

    @staticmethod
    def get_sentences(nlp_doc):
        sentences = []

        for sentence in nlp_doc.sents:
            print(sentence.text)
            sentences.append(sentence.text)

        print("get_sentences: " + str(sentences))
        return sentences

    @staticmethod
    def get_word_occurrences(nlp_doc, filter_stop_words):
        token_dict = {}
        word_occurrences = []

        for token in nlp_doc:
            token_str = str(token)

            if (token.is_stop and filter_stop_words) or token.is_punct:
                continue
            if token_str not in token_dict:
                token_dict[token_str] = 1
            else:
                token_dict[token_str] += 1

        for word, occurrence in token_dict.items():
            word_occurrences.append(WordOccurrence(word, occurrence).__dict__)

        print("get_word_occurrences: " + str(word_occurrences))
        return word_occurrences

    @staticmethod
    def print_spacy_stopwords():
        spacy_stopwords = spacy.lang.de.stop_words.STOP_WORDS
        print('Number of stop words: %d' % len(spacy_stopwords))
        print('First ten stop words: %s' % list(spacy_stopwords)[:10])
