# https://gist.github.com/nitaku/10d0662536f37a087e1b User comment:mfickett
from http.server import HTTPServer, BaseHTTPRequestHandler
from http import HTTPStatus
import json
from NLP import NLP
from SentimentAnalysis import SentimentAnalysis


# Sample blog post data similar to
# https://ordina-jworks.github.io/frontend/2019/03/04/vue-with-typescript.html#4-how-to-write-your-first-component
class _RequestHandler(BaseHTTPRequestHandler):
    # Borrowing from https://gist.github.com/nitaku/10d0662536f37a087e1b
    def _set_headers(self):
        self.send_response(HTTPStatus.OK.value)
        self.send_header('Content-type', 'application/json')
        # Allow requests from any origin, so CORS policies don't
        # prevent local development.
        self.send_header('Access-Control-Allow-Origin', '*')
        self.end_headers()
        self.nlp = NLP()
        self.sentiment_analysis = SentimentAnalysis()

    def do_POST(self):
        try:
            print('do_POST')
            self._set_headers()
            length = int(self.headers.get('content-length'))
            request = self.rfile.read(length)
            message = json.loads(request)
            print('Received: ' + str(message))
            print('error code: ' + str(HTTPStatus.INTERNAL_SERVER_ERROR.value))

            for item in message['items']:
                nlp_doc = self.nlp.get_nlp_doc(item['spokenText'])
                sentences = self.nlp.get_sentences(nlp_doc)
                sentence_sentiments = self.sentiment_analysis.get_sentence_sentiments(sentences)
                word_occurrences = self.nlp.get_word_occurrences(nlp_doc, False)
                item['sentenceSentiments'] = sentence_sentiments
                item['wordOccurrences'] = word_occurrences

            response = json.dumps(message).encode('utf-8')
            print('Sending: ' + str(message))
            self.wfile.write(response)
        except Exception as e:
            print(e)
            self.send_response(HTTPStatus.INTERNAL_SERVER_ERROR.value)
            self.wfile.write(json.dumps({'success': False}).encode('utf-8'))

    def do_OPTIONS(self):
        # Send allow-origin header for preflight POST XHRs.
        self.send_response(HTTPStatus.NO_CONTENT.value)
        self.send_header('Access-Control-Allow-Origin', '*')
        self.send_header('Access-Control-Allow-Methods', 'GET, POST')
        self.send_header('Access-Control-Allow-Headers', 'content-type')
        self.end_headers()


def run_server():
    server_address = ('', 8001)
    httpd = HTTPServer(server_address, _RequestHandler)

    # nlp = NLP()
    # nlp_doc = nlp.get_nlp_doc("Die Dummheit der Unterwerfung blüht in hübschen Farben. Die Dummheit der Unterwerfung blüht in hübschen Farben.")
    # sentences = nlp.get_sentences(nlp_doc)
    # sentence_sentiments = SentimentAnalysis().get_sentence_sentiments(sentences)
    #
    # message = json.loads("{}")
    # message['sentenceSentiments'] = sentence_sentiments
    # message['wordOccurrences'] = nlp.get_word_occurrences(nlp_doc, False)
    # json_string = json.dumps(message).encode('utf-8')
    print('serving at %s:%d' % server_address)
    httpd.serve_forever()


if __name__ == '__main__':
    run_server()
