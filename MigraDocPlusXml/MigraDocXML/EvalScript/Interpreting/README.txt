Stage 1
-------
Stage 1 takes a string and splits it up into its most basic parts, such as string literals, symbols, text and whitespace

For example:
('hello'.Length * 2) + 1

becomes:

[left bracket][string literal][dot][text][whitespace][asterisk][whitespace][text][right bracket][whitespace][plus][whitespace][text]


Stage 2
-------
Stage 2 starts to clean and read meaning into the elements, for example, converting symbols into operators, removing whitespace and converting text into numeric/boolean/null literals or property/function/indexer names

[left bracket][string literal][dot][text][whitespace][asterisk][whitespace][text][right bracket][whitespace][plus][whitespace][text]

becomes:

[left bracket][string literal][dot][property name][multiply][numeric literal][right bracket][plus][numeric literal]


Stage 3
-------
Stage 3 breaks up the code into a tree structure based on brackets and operators

[left bracket][string literal][dot][property name][multiply][numeric literal][right bracket][plus][numeric literal]

becomes:

                                                       [brackets][plus][numeric literal]
                                                           /
                                                          v
                                                    [operation]
                                                       /
                                                      v
[string literal][dot][property name][multiply][numeric literal]


Stage 4
-------
Stage 4 builds the structures into meaningful code objects

                                                       [brackets][plus][numeric literal]
                                                           /
                                                          v
                                                    [operation]
                                                       /
                                                      v
[string literal][dot][property name][multiply][numeric literal]

becomes:

                                          [plus]
                                          /    \
                                         v      v
                                 [multiply]    [numeric literal]
                                  /      \
                                 v        v
                            [chain]      [numeric literal]
                             /
                            v
[string literal],[property name]