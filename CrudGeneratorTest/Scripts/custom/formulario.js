var camposFormCadastro = {
    codigo: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    nome: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    }
};
var colunasTabelaPrincipal = [
    {
        data: 'id',
        name: 'id'
    },
    {
        data: 'codigo',
        name: 'codigo'
    },
    {
        data: 'nome',
        name: 'nome'
    },
    {
        render: renderColunaOpcoes
    }
];
metodoGet = 'Formulario/Get';
metodoInsert = 'Formulario/Insert';

function limparFormCadastro() {
    $('#btnSalvarContinuar').css('display', '');
    $('#btnSalvar').removeData('idRegistro');

    $('#codigo').val('');
    $('#nome').val('');

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'));
}

function carregarFormCadastro(codigo, nome, id) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').data('idRegistro', {
        id: id
    });

    $('#codigo').val(codigo);
    $('#nome').val(nome);

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'));
}

metodoDelete = 'Formulario/Delete';
function colunasTabelaRemocao(objetoTabela) {
    return '{ \'id\': \'' + objetoTabela.id + '\' }';
}

metodoUpdate = 'Formulario/Update';
function colunasTabelaAlteracao(objetoTabela) {
    return '\'' + objetoTabela.codigo + '\', \'' + objetoTabela.nome + '\', \'' + objetoTabela.id + '\'';
}

function montarDadosCadastro() {
    return {
        codigo: $('#codigo').val(),
        nome: $('#nome').val()
    };
}

$(document).ready(function () {
    inicializarTabelaPrincipal($('#tabelaPrincipal'), colunasTabelaPrincipal);

    options.fields = camposFormCadastro;
    $('#formCadastro').bootstrapValidator(options);

    $('#btnNovo').click(function () {
        limparFormCadastro();
    });

    $('#btnSalvar').click(function () {
        var auxUrl;
        var auxData = montarDadosCadastro();

        var auxId = $(this).data('idRegistro');
        if (undefined == auxId) {
            auxUrl = metodoInsert;
        } else {
            auxUrl = metodoUpdate;
            auxData.id = auxId.id;
        }

        salvarRegistro($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), auxUrl, auxData);
    });

    $('#btnSalvarContinuar').click(function () {
        var auxUrl = metodoInsert;
        var auxData = montarDadosCadastro();
        salvarRegistro($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), auxUrl, auxData, undefined, true);
    });

    $('#btnCancelar').click(function () {
        mostrarDvTabelaPrincipal($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), undefined);
    });
});