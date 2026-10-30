#!/usr/bin/env bash

set -euo pipefail

HERE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
mapfile -t PATCHES < <(find "$HERE" -maxdepth 1 -type f -name '[0-9][0-9][0-9][0-9]*.patch' | sort)

if [[ ${#PATCHES[@]} -eq 0 ]]; then
	echo "error: no patch files found in $HERE" >&2
	exit 1
fi

if ! git rev-parse --is-inside-work-tree >/dev/null 2>&1; then
	echo "error: $PWD is not inside a git work tree" >&2
	exit 1
fi

command="${1:---am}"

case "$command" in
	--check)
		if [[ -n "$(git status --porcelain)" ]]; then
			echo "error: the working tree is not clean, commit or stash your changes first" >&2
			exit 1
		fi

		verification="$(mktemp -d)"
		trap 'git worktree remove --force "$verification" >/dev/null 2>&1 || true' EXIT

		git worktree add --quiet --detach "$verification" HEAD
		git -C "$verification" apply "${PATCHES[@]}"
		echo "ok: the patch series applies cleanly to $(git rev-parse --short HEAD)"
		;;
	--no-commit)
		if [[ -n "$(git status --porcelain)" ]]; then
			echo "error: the working tree is not clean, commit or stash your changes first" >&2
			exit 1
		fi
		for patch in "${PATCHES[@]}"; do
			git apply -3 "$patch"
			echo "applied: $(basename "$patch")"
		done
		;;
	--am)
		if [[ -n "$(git status --porcelain)" ]]; then
			echo "error: the working tree is not clean, commit or stash your changes first" >&2
			exit 1
		fi
		git am -3 "${PATCHES[@]}"
		;;
	*)
		echo "usage: $0 [--am|--check|--no-commit]" >&2
		exit 2
		;;
esac
