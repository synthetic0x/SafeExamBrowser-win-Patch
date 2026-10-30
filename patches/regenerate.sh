#!/usr/bin/env bash

set -euo pipefail

HERE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BASE="${1:-origin/master}"

if ! git rev-parse --verify --quiet "$BASE^{commit}" >/dev/null; then
	echo "error: неизвестная база '$BASE'" >&2
	exit 1
fi

mapfile -t REVS < <(git rev-list --reverse "$BASE..HEAD" -- . ':(exclude)patches' ':(exclude).github')

if [[ ${#REVS[@]} -eq 0 ]]; then
	echo "error: между $BASE и HEAD нет патч-коммитов" >&2
	exit 1
fi

find "$HERE" -maxdepth 1 -type f -name '[0-9][0-9][0-9][0-9]*.patch' -delete

number=0
for rev in "${REVS[@]}"; do
	number=$((number + 1))
	git format-patch -1 "$rev" --stdout --no-signature --no-encode-email-headers > "$HERE/$(printf '%04d' "$number").patch"
done

echo "выгрузил $number патчей от $BASE ($(git rev-parse --short "$BASE")):"
find "$HERE" -maxdepth 1 -type f -name '[0-9][0-9][0-9][0-9]*.patch' -printf '  %f\n' | sort
